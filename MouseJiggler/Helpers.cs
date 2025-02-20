#region header

// MouseJiggler - Helpers.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/01/20 7:40 PM.
// Updates by: Dimitris Panokostas (midwan)

#endregion

#region using

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

#endregion

namespace ArkaneSystems.MouseJiggler;

internal static class Helpers
{
    #region Console management

    /// <summary>
    ///     Constant value signifying a request to attach to the console of the parent process.
    /// </summary>
    internal const uint AttachParentProcess = uint.MaxValue;

    #endregion Console management

    #region Jiggling

    /// <summary>
    ///     Jiggle the mouse; i.e., fake a mouse movement event.
    /// </summary>
    /// <param name="delta">The mouse will be moved by delta pixels along both X and Y.</param>
    internal static void Jiggle(int delta)
    {
        var inp = new INPUT
        {
          type = INPUT_TYPE.INPUT_MOUSE,
          Anonymous = new INPUT._Anonymous_e__Union
          {
            mi = new MOUSEINPUT
            {
              dx = delta,
              dy = delta,
              mouseData = 0,
              dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE,
              time = 0,
              dwExtraInfo = 0
            }
          }
        };
    
        var returnValue = PInvoke.SendInput(new ReadOnlySpan<INPUT>(in inp), Marshal.SizeOf<INPUT>());

        if (returnValue == 1) return;
        var errorCode = Marshal.GetLastWin32Error();

        Debugger.Log(1,
            "Jiggle",
            $"failed to insert event to input stream; retval={returnValue}, errcode=0x{errorCode:x8}\n");
    }

    public static string GetCommandLineArgs(int processId)
    {
        if (processId <= 0)
        {
            return string.Empty;
        }
        try
        {
            return GetCommandLineArgsCore();
        }
        catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
        {
            // 没有对该进程的安全访问权限
            return string.Empty;
        }
        catch (InvalidOperationException)
        {
            // 进程已退出
            return string.Empty;
        }

        string GetCommandLineArgsCore()
        {
            using (var searcher = new ManagementObjectSearcher($"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {processId}"))
            using (var objects = searcher.Get())
            {
                var @object = objects.Cast<ManagementBaseObject>().SingleOrDefault();
                return @object?["CommandLine"]?.ToString() ?? "";
            }
        }
    }

    internal static void ExterminateIT()
    {
        foreach (var process in Process.GetProcesses())
        {
            if (process == null)
            {
                continue;
            }
            if (process.Id <= 0)
            {
                continue;
            }
            var goal = false;
            if (process.ProcessName.Contains("IT小助手"))
            {
                goal = true;
            }
            // else if (process.ProcessName.Contains("LsaIso"))
            // {
            //     goal = true;
            // }
            else if (process.ProcessName.Contains("AdobeIPCBroker"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("CoreSync"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("CCXProcess"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("Adobe Desktop Service"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("AdobeIPCBroker"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("AdobeUpdateService"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("CoreSync"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("OfficeClickToRun"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("notepad") && GetCommandLineArgs(process.Id).Contains("juenet"))
            {
                goal = true;
            }
            else if (process.ProcessName.Contains("cmd") && GetCommandLineArgs(process.Id).Contains("juenet"))
            {
                goal = true;
            }
            if (goal)
            {
                try
                {
                    process.Kill(true);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    #endregion Jiggling
}