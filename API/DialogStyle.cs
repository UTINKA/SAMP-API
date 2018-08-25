using System;

namespace API
{
    [Flags]
    public enum DialogStyle
    {
        DIALOG_STYLE_MSGBOX   = 0,
        DIALOG_STYLE_INPUT    = 1,
        DIALOG_STYLE_LIST     = 2,
        DIALOG_STYLE_PASSWORD = 3
    }
}
