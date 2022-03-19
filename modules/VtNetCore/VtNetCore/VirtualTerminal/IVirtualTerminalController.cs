﻿namespace VtNetCore.VirtualTerminal
{
    using VtNetCore.VirtualTerminal.Enums;
    using VtNetCore.XTermParser;

    /// <summary>
    /// Provides the interfaces needed by a terminal in order to manipulate the model.
    /// </summary>
    public interface IVirtualTerminalController
    {

        /// <summary>
        /// Clears the change tracking of the model.
        /// </summary>
        /// <remarks>
        /// This is not really well placed here.
        /// </remarks>
        void ClearChanges();
        bool IsUtf8();
        bool IsVt52Mode();

        void Backspace();
        void Bell();
        void CarriageReturn();
        void ClearScrollingRegion();
        void ClearTab();
        void ClearTabs();
        void DeleteCharacter(int count);
        void DeleteColumn(int count);
        void DeleteLines(int count);
        void DeviceStatusReport();
        void Enable80132Mode(bool enable);
        void Enable132ColumnMode(bool enable);
        void EnableAlternateBuffer();
        void EnableApplicationCursorKeys(bool enable);
        void EnableAutoRepeatKeys(bool enable);
        void EnableBlinkingCursor(bool enable);
        void EnableLeftAndRightMarginMode(bool enable);
        void EnableNationalReplacementCharacterSets(bool enable);
        void EnableNormalBuffer();
        void EnableOriginMode(bool enable);
        void EnableReverseVideoMode(bool enable);
        void EnableReverseWrapAroundMode(bool enable);
        void EnableSmoothScrollMode(bool enable);
        void EnableSgrMouseMode(bool enable);
        void EnableUrxvtMouseMode(bool enabled);
        void EnableWrapAroundMode(bool enable);
        void EraseAbove(bool ignoreProtected);
        void EraseAll(bool ignoreProtected);
        void EraseBelow(bool ignoreProtected);
        void EraseCharacter(int count);
        void EraseLine(bool ignoreProtected);
        void EraseToEndOfLine(bool ignoreProtected);
        void EraseToStartOfLine(bool ignoreProtected);
        void FormFeed();
        void FullReset();
        void InsertBlanks(int count);
        void InsertColumn(int count);
        void InsertLines(int count);
        void InvokeCharacterSetMode(ECharacterSetMode mode);
        void InvokeCharacterSetModeR(ECharacterSetMode mode);
        void MoveCursorRelative(int x, int y);
        void NewLine();
        void PopXTermWindowIcon();
        void PopXTermWindowTitle();
        void ProtectCharacter(int protect);
        void PushXTermWindowIcon();
        void PushXTermWindowTitle();
        void PutChar(char character);
        void PutG2Char(char character);
        void PutG3Char(char character);
        void RepeatLastCharacter(int count);
        void RequestDecPrivateMode(int mode);
        void RequestStatusStringSetConformanceLevel();
        void RequestStatusStringSetProtectionAttribute();
        void ReportCursorPosition();
        void ReportExtendedCursorPosition();
        void ReportRgbBackgroundColor();
        void ReportRgbForegroundColor();
        void RestoreCursor();
        void RestoreEnableNormalBuffer();
        void RestoreEnableSgrMouseMode();
        void RestoreUseCellMotionMouseTracking();
        void RestoreUseHighlightMouseTracking();
        void RestoreBracketedPasteMode();
        void RestoreCursorKeys();
        void ReverseIndex();
        void ReverseTab();
        void SetAutomaticNewLine(bool enable);
        void SaveBracketedPasteMode();
        void SaveCursor();
        void SaveCursorKeys();
        void SaveEnableNormalBuffer();
        void SaveEnableSgrMouseMode();
        void SaveUseCellMotionMouseTracking();
        void SaveUseHighlightMouseTracking();
        void Scroll(int rows);
        void ScrollAcross(int columns);
        void SendDeviceAttributes();
        void SendDeviceAttributesSecondary();
        void SendDeviceAttributesTertiary();
        void SetAbsoluteRow(int line);
        void SetBracketedPasteMode(bool enable);
        void SetCharacterAttribute(int parameter);
        void SetCharacterSet(ECharacterSet characterSet, ECharacterSetMode mode);
        void SetCharacterSize(ECharacterSize size);
        void SetConformanceLevel(int level, bool eightBit);
        void SetCursorPosition(int column, int row);
        void SetCursorStyle(ECursorShape shape, bool blink);
        void SetEndOfGuardedArea();
        void SetErasureMode(bool enabled);
        void SetGuardedAreaTransferMode(bool enabled);
        void SetInsertReplaceMode(EInsertReplaceMode mode);
        void SetIso8613PaletteBackground(int paletteEntry);
        void SetIso8613PaletteForeground(int paletteEntry);
        void SetLatin1();
        void SetLeftAndRightMargins(int left, int right);
        void SetKeypadType(EKeypadType type);
        void SetRgbBackgroundColor(int red, int green, int blue);
        void SetRgbBackgroundColor(string xParseColor);
        void SetRgbForegroundColor(int red, int green, int blue);
        void SetRgbForegroundColor(string xParseColor);
        void SetScrollingRegion(int top, int bottom);
        void SetSendFocusInAndFocusOutEvents(bool enabled);
        void SetStartOfGuardedArea();
        void SetUseAllMouseTracking(bool enabled);
        void SetUTF8();
        void SetUtf8MouseMode(bool enabled);
        void SetVt52AlternateKeypadMode(bool enabled);
        void SetVt52GraphicsMode(bool enabled);
        void SetVt52Mode(bool enabled);
        void SetWindowTitle(string title);
        void SetX10SendMouseXYOnButton(bool enabled);
        void SetX11SendMouseXYOnButton(bool enabled);
        void ShiftIn();
        void ShiftOut();
        void SingleShiftSelectG2();
        void SingleShiftSelectG3();
        void ShowCursor(bool show);
        void Tab();
        void TabSet();
        void UseCellMotionMouseTracking(bool enable);
        void UseHighlightMouseTracking(bool enable);
        void VerticalTab();
        void Vt52EnterAnsiMode();
        void Vt52Identify();
        void XTermDeiconifyWindow();
        void XTermFullScreenEnter();
        void XTermFullScreenExit();
        void XTermFullScreenToggle();
        void XTermIconifyWindow();
        void XTermLowerToBottom();
        void XTermMaximizeWindow(bool horizontally, bool vertically);
        void XTermMoveWindow(int x, int y);
        void XTermRaiseToFront();
        void XTermRefreshWindow();
        void XTermReport(XTermReportType reportType);
        void XTermResizeTextArea(int columns, int rows);
        void XTermResizeWindow(int width, int height);
    }
}
