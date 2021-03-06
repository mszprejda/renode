//
// Copyright (c) 2010-2017 Antmicro
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//
using Antmicro.Renode.Core;
using Antmicro.Renode.Exceptions;
#if !PLATFORM_WINDOWS
using System;
using Antmicro.Renode.Utilities;
using AntShell.Terminal;
using Mono.Unix;
using System.IO;
using Antmicro.Renode.Backends.Terminals;
#endif

namespace Antmicro.Renode.Backends.Terminals
{
    public static class UartPtyTerminalExtensions
    {
        public static void CreateUartPtyTerminal(this Emulation emulation, string name, string fileName, bool forceCreate = false)
        {
#if !PLATFORM_WINDOWS
            emulation.ExternalsManager.AddExternal(new UartPtyTerminal(fileName, forceCreate), name);
#else
            throw new RecoverableException("Creating UartPtyTerminal is not supported on Windows.");
#endif
        }
    }

#if !PLATFORM_WINDOWS
    public class UartPtyTerminal : BackendTerminal, IDisposable
    {
        public UartPtyTerminal(string linkName, bool forceCreate = false)
        {
            var ptyUnixStream = new PtyUnixStream();

            io = new IOProvider { Backend = new StreamIOSource(ptyUnixStream) };
            io.ByteRead += b => CallCharReceived((byte)b);

            if(File.Exists(linkName))
            {
                if(!forceCreate)
                {
                    throw new RecoverableException(string.Format("File `{0}' already exists. Use forceCreate to overwrite it.", linkName));
                }

                try
                {
                    File.Delete(linkName);
                }
                catch(Exception e)
                {
                    throw new RecoverableException(string.Format("There was an error when removing existing `{0}' symlink: {1}", linkName, e.Message));
                }
            }
            try
            {
                var slavePtyFile = new UnixFileInfo(ptyUnixStream.SlaveName);
                symlink = slavePtyFile.CreateSymbolicLink(linkName);
            }
            catch(Exception e)
            {
                throw new RecoverableException(string.Format("There was an error when when creating a symlink `{0}': {1}", linkName, e.Message));
            }
        }

        public void Dispose()
        {
            io.Dispose();
            try
            {
                symlink.Delete();
            }
            catch(Exception e)
            {
                throw new RecoverableException(string.Format("There was an error when removing symlink `{0}': {1}", symlink.Name, e.Message));
            }
        }

        public override void WriteChar(byte value)
        {
            io.Write(value);
        }

        private readonly IOProvider io;
        private readonly UnixSymbolicLinkInfo symlink;
    }
#endif
}

