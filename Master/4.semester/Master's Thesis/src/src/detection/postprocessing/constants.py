# File: constants.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains commonly used constant values to avoid typo mistakes.

layout = "layout"
special = "special"

# layouts
qwerty = "qwerty"
alphabet = "alphabet"
pinpad = "pinpad"
pinpad_column = "pinpad_column"
number_line = "number line"
char_line = "char line"

# special keys
space = "space"
enter = "enter"
shift = "shift"
backspace = "backspace"
capslock = "caps"  # sometimes it's just caps/CapsLk, the discriminative part of it is caps, lock can be in NumLock etc.
tab = "tab"

keywords = [space, enter, shift, backspace, capslock, tab]
alternative_keywords = ["english", "return", "cap", "backsp", "bksp"]

# mode options
mode = "mode"
mode_shift = "modeshift"
modes = ["ABC", "abc", "123", "&123", "?123", "12#", '12!?', "!#1", "+!=", "=\\<", "#+=",
         "!&#", "{&=", "#$%", "!#$", "!@#", "#=@", "?=&", "#@!", "@#&", "<\\{", "SYM", "Sym"]

# pagination
page = "page"
pages = ["1/2", "2/2"]  # there are almost only 2 pages if any
