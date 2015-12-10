﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal sealed class CoreAudioSession : IAudioSession, IAudioSessionEvents
    {

        private readonly IAudioSessionControl2 _audioSessionControl;
        private readonly ISimpleAudioVolume _simpleAudioVolume;
        private readonly IAudioMeterInformation _meterInformation;

        private string _fileDescription;
        private double _volume;
        private string _id;
        private int _processId;
        private string _displayName;
        private string _iconPath;
        private bool _isSystemSession;
        private AudioSessionState _state;
        private string _executablePath;
        private readonly AsyncBroadcaster<SessionStateChangedArgs> _stateChanged;
        private readonly AsyncBroadcaster<SessionDisconnectedArgs> _disconnected;
        private readonly AsyncBroadcaster<SessionVolumeChangedArgs> _volumeChanged;
        private readonly AsyncBroadcaster<SessionMuteChangedArgs> _muteChanged;
        private readonly AsyncBroadcaster<PeakValueChangedArgs> _peakValueChanged;
        private bool _isMuted;
        private Timer _timer;
        private bool _isDisposed;

        public IObservable<SessionVolumeChangedArgs> VolumeChanged
        {
            get { return _volumeChanged.AsObservable(); }
        }

        public IObservable<PeakValueChangedArgs> PeakValueChanged
        {
            get
            {
                return _peakValueChanged;
            }
        }

        public IObservable<SessionMuteChangedArgs> MuteChanged
        {
            get { return _muteChanged.AsObservable(); }
        }

        public IObservable<SessionStateChangedArgs> StateChanged
        {
            get
            {
                return _stateChanged.AsObservable();
            }
        }

        public IObservable<SessionDisconnectedArgs> Disconnected
        {
            get
            {
                return _disconnected.AsObservable();
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public int ProcessId
        {
            get
            {
                return _processId;
            }
        }

        public string DisplayName
        {
            get
            {
                return String.IsNullOrWhiteSpace(_displayName) ? _fileDescription : _displayName;
            }
        }

        public string IconPath
        {
            get
            {
                return _iconPath;
            }
        }

        public string ExecutablePath
        {
            get
            {
                return _executablePath;
            }
        }

        public bool IsSystemSession
        {
            get
            {
                return _isSystemSession;
            }
        }

        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (_isDisposed)
                    return;

                ComThread.Invoke(() => _simpleAudioVolume.SetMasterVolume((float)(value / 100), Guid.Empty));
                _volume = value;
                OnVolumeChanged(_volume);
            }
        }

        public bool IsMuted
        {
            get
            {
                return _isMuted;
            }
            set
            {
                if (_isMuted == value || _isDisposed)
                    return;

                ComThread.Invoke(() => _simpleAudioVolume.SetMute(value, Guid.Empty));

                _isMuted = value;
                OnMuteChanged(_isMuted);
            }
        }

        public AudioSessionState SessionState
        {
            get
            {
                return _state;
            }
        }

        public CoreAudioSession(IAudioSessionControl control)
        {
            ComThread.Assert();

            // ReSharper disable once SuspiciousTypeConversion.Global
            _audioSessionControl = control as IAudioSessionControl2;

            // ReSharper disable once SuspiciousTypeConversion.Global
            _simpleAudioVolume = control as ISimpleAudioVolume;

            // ReSharper disable once SuspiciousTypeConversion.Global
            _meterInformation = control as IAudioMeterInformation;

            if (_audioSessionControl == null || _simpleAudioVolume == null)
                throw new InvalidComObjectException("control");

            if (_meterInformation != null)
            {
                //start a timer to poll for peak value changes
                _timer = new Timer(Timer_UpdatePeakValue, null, 0, 20);
            }

            _stateChanged = new AsyncBroadcaster<SessionStateChangedArgs>();
            _disconnected = new AsyncBroadcaster<SessionDisconnectedArgs>();
            _volumeChanged = new AsyncBroadcaster<SessionVolumeChangedArgs>();
            _muteChanged = new AsyncBroadcaster<SessionMuteChangedArgs>();
            _peakValueChanged = new AsyncBroadcaster<PeakValueChangedArgs>();

            _audioSessionControl.RegisterAudioSessionNotification(this);

            RefreshProperties();
            RefreshVolume();
        }

        private void Timer_UpdatePeakValue(object state)
        {
            float peakValue = 0;
            ComThread.Invoke(() =>
            {
                if (_isDisposed)
                    return;

                try
                {
                    _meterInformation.GetPeakValue(out peakValue);
                }
                catch (InvalidComObjectException)
                {
                    //ignored - usually means the com object has been released, but the timer is still ticking
                    if (_timer != null)
                        _timer.Dispose();
                }
            });
            OnPeakValueChanged(peakValue * 100);
        }

        ~CoreAudioSession()
        {
            Dispose();
        }

        private void RefreshVolume()
        {
            if (_isDisposed)
                return;

            ComThread.Invoke(() =>
            {
                float vol;
                _simpleAudioVolume.GetMasterVolume(out vol);
                _volume = vol * 100;

                bool isMuted;
                _simpleAudioVolume.GetMute(out isMuted);

                _isMuted = isMuted;
            });
        }

        private void RefreshProperties()
        {
            if (_isDisposed)
                return;

            ComThread.Invoke(() =>
            {
                _isSystemSession = _audioSessionControl.IsSystemSoundsSession() == 0;
                _audioSessionControl.GetDisplayName(out _displayName);

                _audioSessionControl.GetIconPath(out _iconPath);

                EAudioSessionState state;
                _audioSessionControl.GetState(out state);
                _state = state.AsAudioSessionState();

                uint processId;
                _audioSessionControl.GetProcessId(out processId);
                _processId = (int)processId;

                _audioSessionControl.GetSessionIdentifier(out _id);

                try
                {
                    if (ProcessId > 0)
                    {
                        var proc = Process.GetProcessById(ProcessId);
                        _executablePath = proc.MainModule.FileName;
                        _fileDescription = proc.MainModule.FileVersionInfo.FileDescription;
                    }
                }
                catch
                {
                    _fileDescription = "";
                }
            });
        }

        int IAudioSessionEvents.OnDisplayNameChanged(string displayName, ref Guid eventContext)
        {
            _displayName = displayName;
            return 0;
        }

        int IAudioSessionEvents.OnIconPathChanged(string iconPath, ref Guid eventContext)
        {
            _iconPath = iconPath;
            return 0;
        }

        int IAudioSessionEvents.OnSimpleVolumeChanged(float volume, bool isMuted, ref Guid eventContext)
        {
            if (Math.Abs(_volume - volume * 100) > 0)
            {
                _volume = volume * 100;
                OnVolumeChanged(_volume);
            }

            if (isMuted != _isMuted)
            {
                _isMuted = isMuted;
                OnMuteChanged(_isMuted);
            }

            return 0;
        }

        int IAudioSessionEvents.OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex, ref Guid eventContext)
        {
            return 0;
        }

        int IAudioSessionEvents.OnGroupingParamChanged(ref Guid groupingId, ref Guid eventContext)
        {
            return 0;
        }

        int IAudioSessionEvents.OnSessionDisconnected(EAudioSessionDisconnectReason disconnectReason)
        {
            OnDisconnected();
            return 0;
        }

        int IAudioSessionEvents.OnStateChanged(EAudioSessionState state)
        {
            _state = state.AsAudioSessionState();
            OnStateChanged(state);
            return 0;
        }

        private void OnVolumeChanged(double volume)
        {
            _volumeChanged.OnNext(new SessionVolumeChangedArgs(this, volume));
        }

        private void OnStateChanged(EAudioSessionState state)
        {
            _stateChanged.OnNext(new SessionStateChangedArgs(this, state.AsAudioSessionState()));
        }

        private void OnDisconnected()
        {
            _disconnected.OnNext(new SessionDisconnectedArgs(this));
        }

        private void OnMuteChanged(bool isMuted)
        {
            _muteChanged.OnNext(new SessionMuteChangedArgs(this, isMuted));
        }

        private void OnPeakValueChanged(double peakValue)
        {
            _peakValueChanged.OnNext(new PeakValueChangedArgs(this, peakValue));
        }

        public void Dispose()
        {
            _isDisposed = true;

            if (_timer != null)
                _timer.Dispose();

            _stateChanged.Dispose();
            _disconnected.Dispose();
            _volumeChanged.Dispose();

            _audioSessionControl.UnregisterAudioSessionNotification(this);

            Marshal.FinalReleaseComObject(_audioSessionControl);

            GC.SuppressFinalize(this);
        }
    }
}
