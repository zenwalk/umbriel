// <copyright file="WindowFunctions.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-10-18</date>
// <summary>WindowFunctions class file</summary>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// user32.dll window functions
/// </summary>
public class WindowFunctions
{
    [DllImport("user32", SetLastError = true)]
    public static extern int GetWindowThreadProcessId(int hwnd, ref int lProcessId);

     [DllImport("user32.dll")]
    private static extern int EnumWindows(CallBackPtr callPtr, IntPtr lPar);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool BringWindowToTop(IntPtr hWnd);

    private static List<IntPtr> mHandles = new List<IntPtr>();

    private delegate bool CallBackPtr(IntPtr hWnd, IntPtr lParam);

    public static List<IntPtr> GetWindows()
    {
        mHandles.Clear();
        CallBackPtr callBackPtr = new CallBackPtr(Callback);
        EnumWindows(callBackPtr, IntPtr.Zero);
        return mHandles;
    }

    public static int GetProcessThreadFromWindow(int hwnd)
    {
        int procid = 0;
        int threadid = GetWindowThreadProcessId(hwnd, ref procid);
        return procid;
    }

    public static string GetText(IntPtr hWnd)
    {
        // Allocate correct string length first
        int length = GetWindowTextLength(hWnd);
        StringBuilder sb = new StringBuilder(length + 1);
        GetWindowText(hWnd, sb, sb.Capacity);
        return sb.ToString();
    }

    private static bool Callback(IntPtr hWnd, IntPtr lParam)
    {
        mHandles.Add(hWnd);
        return true;
    }
}