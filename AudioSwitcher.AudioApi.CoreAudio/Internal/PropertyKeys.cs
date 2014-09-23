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

using System;

// This has been modified for use by Audio Switcher

namespace AudioSwitcher.AudioApi.CoreAudio
{
    /// <summary>
    ///     Property Keys
    /// </summary>
    internal static class PropertyKeys
    {
        /// <summary>
        ///     PKEY_DeviceInterface_FriendlyName
        /// </summary>
        public static readonly PropertyKey PKEY_DeviceInterface_FriendlyName =
            new PropertyKey(new Guid(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22), 2);

        /// <summary>
        ///     PKEY_AudioEndpoint_FormFactor
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_FormFactor =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 0);

        /// <summary>
        ///     PKEY_AudioEndpoint_ControlPanelPageProvider
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_ControlPanelPageProvider =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 1);

        /// <summary>
        ///     PKEY_AudioEndpoint_Association
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_Association =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 2);

        /// <summary>
        ///     PKEY_AudioEndpoint_PhysicalSpeakers
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_PhysicalSpeakers =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 3);

        /// <summary>
        ///     PKEY_AudioEndpoint_GUID
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_GUID =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 4);

        /// <summary>
        ///     PKEY_AudioEndpoint_Disable_SysFx
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_Disable_SysFx =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 5);

        /// <summary>
        ///     PKEY_AudioEndpoint_FullRangeSpeakers
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_FullRangeSpeakers =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 6);

        /// <summary>
        ///     PKEY_AudioEndpoint_Supports_EventDriven_Mode
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_Supports_EventDriven_Mode =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 7);

        /// <summary>
        ///     PKEY_AudioEndpoint_JackSubType
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEndpoint_JackSubType =
            new PropertyKey(new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e), 8);

        /// <summary>
        ///     PKEY_AudioEngine_DeviceFormat
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEngine_DeviceFormat =
            new PropertyKey(new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c), 0);

        /// <summary>
        ///     PKEY_AudioEngine_OEMFormat
        /// </summary>
        public static readonly PropertyKey PKEY_AudioEngine_OEMFormat =
            new PropertyKey(new Guid(0xe4870e26, 0x3cc5, 0x4cd2, 0xba, 0x46, 0xca, 0xa, 0x9a, 0x70, 0xed, 0x4), 3);

        /// <summary>
        ///     PKEY _Device_FriendlyName
        /// </summary>
        public static readonly PropertyKey PKEY_Device_FriendlyName =
            new PropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 14);

        /// <summary>
        ///     PKEY _Device_Description
        /// </summary>
        public static readonly PropertyKey PKEY_Device_Description =
            new PropertyKey(new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 2);

        /// <summary>
        ///     PKEY _Device_Icon
        /// </summary>
        public static readonly PropertyKey PKEY_Device_Icon =
            new PropertyKey(new Guid(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x08, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66), 12);

        /// <summary>
        ///     PKEY _System_Name
        /// </summary>
        public static readonly PropertyKey PKEY_System_Name =
            new PropertyKey(new Guid(0xb3f8fa53, 0x0004, 0x438e, 0x90, 0x03, 0x51, 0xa4, 0x6e, 0x13, 0x9b, 0xfc), 6);
    }
}