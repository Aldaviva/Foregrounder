using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Foregrounder;

/// <summary>
/// Bring a window in your process to the foreground, even if a different process's window is currently in the foreground.
/// </summary>
/// <remarks>
/// <para>This algorithm is by Carl Scarlett (Zodman) and Joseph Cooney.</para>
/// <para>https://stackoverflow.com/a/11552906/979493</para>
/// </remarks>
public static class Foregrounder {

    private const uint SwpNosize     = 0x0001;
    private const uint SwpNomove     = 0x0002;
    private const uint SwpShowwindow = 0x0040;

    /// <summary>
    /// Bring the given WPF window to the foreground. If it is minimized, it will be restored.
    /// </summary>
    /// <param name="wpfWindow">The WPF window to bring to the foreground.</param>
    public static void BringToForeground(Window wpfWindow) {
        BringToForeground(new WindowInteropHelper(wpfWindow).Handle);

        wpfWindow.Show();
        wpfWindow.Activate();
    }

    /// <summary>
    /// Bring the given Windows Forms window to the foreground. If it is minimized, it will be restored.
    /// </summary>
    /// <param name="formsWindow">The Windows Forms window to bring to the foreground.</param>
    public static void BringToForeground(Form formsWindow) {
        BringToForeground(formsWindow.Handle);

        formsWindow.Show();
        formsWindow.Activate();
    }

    /// <summary>
    /// Bring the given UI Automation Element window to the foreground. If it is minimized, it will be restored.
    /// </summary>
    /// <param name="automationElement">The UI Automation Element to bring to the foreground.</param>
    public static void BringToForeground(AutomationElement automationElement) {
        BringToForeground(new IntPtr(automationElement.Current.NativeWindowHandle));
    }

    /// <summary>
    /// Bring the given window to the foreground. If it is minimized, it will be restored.
    /// </summary>
    /// <remarks>
    /// <para>This algorithm is by Carl Scarlett (Zodman) and Joseph Cooney.</para>
    /// <para>https://stackoverflow.com/a/11552906/979493</para>
    /// </remarks>
    /// <param name="windowHandle">The window handle (<c>HWND</c>) to bring to the foreground.</param>
    public static void BringToForeground(IntPtr windowHandle) {
        uint selfWindowThreadId = User32.GetWindowThreadProcessId(windowHandle, IntPtr.Zero);

        //Get the thread ID for the foreground window
        IntPtr foregroundWindow         = User32.GetForegroundWindow();
        uint   foregroundWindowThreadId = User32.GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);

        //Attach the input processing mechanism of the foreground window's thread to that of this window's thread
        User32.AttachThreadInput(foregroundWindowThreadId, selfWindowThreadId, true);

        //Set this window to be on top of the z-order, displaying it but neither moving nor resizing it
        User32.SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0, SwpNosize | SwpNomove | SwpShowwindow);

        //Detach the foreground window's thread from the current window's thread
        User32.AttachThreadInput(foregroundWindowThreadId, selfWindowThreadId, false);

        //If the window is minimized, restore it
        User32.WindowPlacement oldWindowPlacement = User32.WindowPlacement.Create();
        User32.GetWindowPlacement(windowHandle, ref oldWindowPlacement);
        if (oldWindowPlacement.ShowCmd == User32.ShowWindowCommands.ShowMinimized) {
            User32.WindowPlacement newWindowPlacement = User32.WindowPlacement.Create();
            newWindowPlacement.ShowCmd = User32.ShowWindowCommands.Restore;
            User32.SetWindowPlacement(windowHandle, ref newWindowPlacement);
        }
    }

}