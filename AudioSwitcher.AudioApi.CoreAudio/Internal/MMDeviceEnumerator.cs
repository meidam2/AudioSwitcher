﻿/*
  LICENSE
  -------
  Copyright (C) 2007 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/
// updated for use in NAudio

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    /// <summary>
    ///     MM Device Enumerator
    /// </summary>
    internal class MMDeviceEnumerator
    {
        private IMMDeviceEnumerator _realEnumerator;

        /// <summary>
        ///     Creates a new MM Device Enumerator
        /// </summary>
        public MMDeviceEnumerator()
        {
#if !NETFX_CORE
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }
#endif
            ComThread.Invoke(() => { _realEnumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator; });
        }

        /// <summary>
        ///     Enumerate Audio Endpoints
        /// </summary>
        /// <param name="eDataFlow">Desired DeviceType</param>
        /// <param name="dwStateMask">State Mask</param>
        /// <returns>Device Collection</returns>
        public MMDeviceCollection EnumerateAudioEndPoints(EDataFlow eDataFlow, EDeviceState dwStateMask)
        {
            return ComThread.Invoke(() =>
            {
                IMMDeviceCollection result;
                Marshal.ThrowExceptionForHR(_realEnumerator.EnumAudioEndpoints(eDataFlow, dwStateMask, out result));
                return new MMDeviceCollection(result);
            });
        }

        /// <summary>
        ///     Get Default Endpoint
        /// </summary>
        /// <param name="eDataFlow">Data Flow</param>
        /// <param name="role">Role</param>
        /// <returns>Device</returns>
        public MMDevice GetDefaultAudioEndpoint(EDataFlow eDataFlow, ERole role)
        {
            return ComThread.Invoke(() =>
            {
                try
                {
                    IMMDevice device;
                    Marshal.ThrowExceptionForHR(_realEnumerator.GetDefaultAudioEndpoint(eDataFlow, role, out device));
                    return new MMDevice(device);
                }
                catch
                {
                    Debug.WriteLine("Device does not exist");
                }

                return null;
            });
        }

        public string GetDefaultAudioEndpointId(EDataFlow eDataFlow, Role role)
        {
            try
            {
                IMMDevice device;
                Marshal.ThrowExceptionForHR(_realEnumerator.GetDefaultAudioEndpoint(eDataFlow, role.AsERole(),
                    out device));
                string result;
                Marshal.ThrowExceptionForHR(device.GetId(out result));
                return result;
            }
            catch
            {
                Debug.WriteLine("Device does not exist");
            }

            return null;
        }

        /// <summary>
        ///     Get device by ID
        /// </summary>
        /// <param name="ID">Device ID</param>
        /// <returns>Device</returns>
        public MMDevice GetDevice(string ID)
        {
            return ComThread.Invoke(() =>
            {
                IMMDevice device;
                Marshal.ThrowExceptionForHR(_realEnumerator.GetDevice(ID, out device));
                return new MMDevice(device);
            });
        }

        public void RegisterEndpointNotificationCallback(IMMNotificationClient client)
        {
            ComThread.Invoke(() => { _realEnumerator.RegisterEndpointNotificationCallback(client); });
        }

        public void UnregisterEndpointNotificationCallback(IMMNotificationClient client)
        {
            ComThread.Invoke(() => { _realEnumerator.UnregisterEndpointNotificationCallback(client); });
        }
    }
}