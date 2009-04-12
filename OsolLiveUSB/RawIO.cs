/*
 * CDDL HEADER START
 *
 * The contents of this file are subject to the terms of the
 * Common Development and Distribution License (the "License").
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the license at usr/src/OPENSOLARIS.LICENSE
 * or http://www.opensolaris.org/os/licensing.
 * See the License for the specific language governing permissions
 * and limitations under the License.
 *
 * When distributing Covered Code, include this CDDL HEADER in each
 * file and include the License file at usr/src/OPENSOLARIS.LICENSE.
 * If applicable, add the following below this CDDL HEADER, with the
 * fields enclosed by brackets "[]" replaced with your own identifying
 * information: Portions Copyright [yyyy] [name of copyright owner]
 *
 * CDDL HEADER END
 */

/*      Copyright (c) 2009 Hiroshi Chonan <chonan@pid0.org> */
/*        All Rights Reserved   */

using System;
using System.Runtime.InteropServices;

namespace OsolLiveUSB
{
    class RawIO
    {
        [DllImport("Kernel32.dll", EntryPoint="CreateFile", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr CreateFile(
            string lpFileName,              
            uint dwDesiredAccess,            
            uint dwShareMode,                
            IntPtr lpSecurityAttributes,     
            uint dwCreationDisposition,      
            uint dwFlagsAndAttributes,       
            IntPtr hTemplateFile             
            );

        [DllImport("Kernel32.dll", EntryPoint = "CloseHandle", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(
            IntPtr hObject                  
            );

        [DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(
            IntPtr hFile,                    
            uint dwIoControlCode,            
            byte[] lpInBuffer,               
            uint nInBufferSize,              
            byte[] lpOutBuffer,              
            uint nOutBufferSize,             
            out uint lpBytesReturned,        
            IntPtr lpOverlapped             
            );

        [DllImport("Kernel32.dll", EntryPoint="ReadFile", CharSet=CharSet.Auto, SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFile(
            IntPtr hFile,                   
            byte[] lpBuffer,                
            uint nNumberOfBytesToRead,      
            out uint lpNumberOfBytesRead,   
            IntPtr lpOverlapped             
        );

        [DllImport("Kernel32.dll", EntryPoint = "WriteFile", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(
            IntPtr hFile,                   
            byte[] lpBuffer,                
            uint nNumberOfBytesToWrite,     
            out uint lpNumberOfBytesWriten, 
            IntPtr lpOverlapped             
        );

        [DllImport("Kernel32.dll", EntryPoint = "SetFilePointer", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint SetFilePointer(
            IntPtr hFile,
            uint lDistanceToMove,
            ref uint lpDistanceToMoveHigh,
            uint dwMoveMethod
        );

        private enum FileAccess : uint
        {
            FILE_ANY_ACCESS                     = 0x0000,
            FILE_SPECIAL_ACCESS                 = FILE_ANY_ACCESS,
            FILE_READ_ACCESS                    = 0x0001,
            FILE_WRITE_ACCESS                   = 0x0002,
        }
        private enum IoMethod : uint
        {
            METHOD_BUFFERED                     = 0x00000000,
            METHOD_IN_DIRECT                    = 0x00000001,
            METHOD_OUT_DIRECT                   = 0x00000002,
            METHOD_NEITHER                      = 0x00000003
        }

        public enum DesiredAccess : uint {
            GENERIC_READ                        = 0x80000000,
            GENERIC_WRITE                       = 0x40000000,
            GENERIC_EXECUTE                     = 0x20000000,
            GENERIC_ALL                         = 0x10000000,
        }
        public enum ShareMode : uint {
            FILE_SHARE_READ                     = 0x00000001,
            FILE_SHARE_WRITE                    = 0x00000002,
            FILE_SHARE_DELETE                   = 0x00000004,  
        }
        public enum CreationDisposition : uint {
            CREATE_NEW                          = 1,
            CREATE_ALWAYS                       = 2,
            OPEN_EXISTING                       = 3,
            OPEN_ALWAYS                         = 4,
            TRUNCATE_EXISTING                   = 5,
        }
        public enum FlagsAndAttributes : uint {
            FILE_ATTRIBUTE_READONLY             = 0x00000001,
            FILE_ATTRIBUTE_HIDDEN               = 0x00000002,
            FILE_ATTRIBUTE_SYSTEM               = 0x00000004,
            FILE_ATTRIBUTE_DIRECTORY            = 0x00000010,
            FILE_ATTRIBUTE_ARCHIVE              = 0x00000020,
            FILE_ATTRIBUTE_DEVICE               = 0x00000040,
            FILE_ATTRIBUTE_NORMAL               = 0x00000080,
            FILE_ATTRIBUTE_TEMPORARY            = 0x00000100,
            FILE_ATTRIBUTE_SPARSE_FILE          = 0x00000200,
            FILE_ATTRIBUTE_REPARSE_POINT        = 0x00000400,
            FILE_ATTRIBUTE_COMPRESSED           = 0x00000800,
            FILE_ATTRIBUTE_OFFLINE              = 0x00001000,
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED  = 0x00002000,
            FILE_ATTRIBUTE_ENCRYPTED            = 0x00004000,
        }

        public enum IoControlCode : uint {
            // IOCTL_DISK
            IOCTL_DISK_GET_DRIVE_GEOMETRY       = 0x00070000,
            IOCTL_DISK_GET_PARTITION_INFO       = 0x00074004,
            IOCTL_DISK_SET_PARTITION_INFO       = 0x0007C008,
            IOCTL_DISK_GET_DRIVE_LAYOUT         = 0x0007400C,
            IOCTL_DISK_SET_DRIVE_LAYOUT         = 0x0007C010,
            IOCTL_DISK_VERIFY                   = 0x00070014,
            IOCTL_DISK_FORMAT_TRACKS            = 0x0007C018,
            IOCTL_DISK_REASSIGN_BLOCKS          = 0x0007C01C,
            IOCTL_DISK_PERFORMANCE              = 0x00070020,
            IOCTL_DISK_IS_WRITABLE              = 0x00070024,
            IOCTL_DISK_LOGGING                  = 0x00070028,
            IOCTL_DISK_FORMAT_TRACKS_EX         = 0x0007C02C,
            IOCTL_DISK_HISTOGRAM_STRUCTURE      = 0x00070030,
            IOCTL_DISK_HISTOGRAM_DATA           = 0x00070034,
            IOCTL_DISK_HISTOGRAM_RESET          = 0x00070038,
            IOCTL_DISK_REQUEST_STRUCTURE        = 0x0007003C,
            IOCTL_DISK_REQUEST_DATA             = 0x00070040,
            IOCTL_DISK_PERFORMANCE_OFF          = 0x00070060,
            IOCTL_DISK_CONTROLLER_NUMBER        = 0x00070044,
            SMART_GET_VERSION                   = 0x00074080,
            SMART_SEND_DRIVE_COMMAND            = 0x0007C084,
            SMART_RCV_DRIVE_DATA                = 0x0007C088,
            IOCTL_DISK_GET_PARTITION_INFO_EX    = 0x00070048,
            IOCTL_DISK_SET_PARTITION_INFO_EX    = 0x0007C04C,
            IOCTL_DISK_GET_DRIVE_LAYOUT_EX      = 0x00070050,
            IOCTL_DISK_SET_DRIVE_LAYOUT_EX      = 0x0007C054,
            IOCTL_DISK_CREATE_DISK              = 0x0007C058,
            IOCTL_DISK_GET_LENGTH_INFO          = 0x0007405C,
            IOCTL_DISK_GET_DRIVE_GEOMETRY_EX    = 0x000700A0,
            IOCTL_DISK_REASSIGN_BLOCKS_EX       = 0x0007C0A4,
            IOCTL_DISK_UPDATE_DRIVE_SIZE        = 0x0007C0C8,
            IOCTL_DISK_GROW_PARTITION           = 0x0007C0D0,
            IOCTL_DISK_GET_CACHE_INFORMATION    = 0x000740D4,
            IOCTL_DISK_SET_CACHE_INFORMATION    = 0x0007C0D8,
            OBSOLETE_DISK_GET_WRITE_CACHE_STATE = 0x000740DC,
            IOCTL_DISK_DELETE_DRIVE_LAYOUT      = 0x0007C100,
            IOCTL_DISK_UPDATE_PROPERTIES        = 0x00070140,
            IOCTL_DISK_FORMAT_DRIVE             = 0x0007C3CC,
            IOCTL_DISK_SENSE_DEVICE             = 0x000703E0,
            IOCTL_DISK_CHECK_VERIFY             = 0x00074800,
            IOCTL_DISK_MEDIA_REMOVAL            = 0x00074804,
            IOCTL_DISK_EJECT_MEDIA              = 0x00074808,
            IOCTL_DISK_LOAD_MEDIA               = 0x0007480C,
            IOCTL_DISK_RESERVE                  = 0x00074810,
            IOCTL_DISK_RELEASE                  = 0x00074814,
            IOCTL_DISK_FIND_NEW_DEVICES         = 0x00074818,
            IOCTL_DISK_GET_MEDIA_TYPES          = 0x00070C00,
            // IOCTL_STORAGE
            IOCTL_STORAGE_CHECK_VERIFY          = 0x002D4800,
            IOCTL_STORAGE_CHECK_VERIFY2         = 0x002D0800,
            IOCTL_STORAGE_MEDIA_REMOVAL         = 0x002D4804,
            IOCTL_STORAGE_EJECT_MEDIA           = 0x002D4808,
            IOCTL_STORAGE_LOAD_MEDIA            = 0x002D480C,
            IOCTL_STORAGE_LOAD_MEDIA2           = 0x002D080C,
            IOCTL_STORAGE_RESERVE               = 0x002D4810,
            IOCTL_STORAGE_RELEASE               = 0x002D4814,
            IOCTL_STORAGE_FIND_NEW_DEVICES      = 0x002D4818,
            IOCTL_STORAGE_EJECTION_CONTROL      = 0x002D0940,
            IOCTL_STORAGE_MCN_CONTROL           = 0x002D0944,
            IOCTL_STORAGE_GET_MEDIA_TYPES       = 0x002D0C00,
            IOCTL_STORAGE_GET_MEDIA_TYPES_EX    = 0x002D0C04,
            IOCTL_STORAGE_GET_MEDIA_SERIAL_NUMBER = 0x002D0C10,
            IOCTL_STORAGE_GET_HOTPLUG_INFO      = 0x002D0C14,
            IOCTL_STORAGE_SET_HOTPLUG_INFO      = 0x002DCC18,
            IOCTL_STORAGE_RESET_BUS             = 0x002D5000,
            IOCTL_STORAGE_RESET_DEVICE          = 0x002D5004,
            IOCTL_STORAGE_BREAK_RESERVATION     = 0x002D5014,
            IOCTL_STORAGE_GET_DEVICE_NUMBER     = 0x002D1080,
            IOCTL_STORAGE_PREDICT_FAILURE       = 0x002D1100,
            IOCTL_STORAGE_READ_CAPACITY         = 0x002D5140,
        }

        public enum MoveMethod : uint {
            FileBegin   = 0,
            FileCurrent = 1,
            FileEnd     = 2,
        }

    }
}
