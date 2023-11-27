# File: layouts.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Contains predefined supported layout values.

qwerty = ["qwertyuiop", "asdfghjkl", "zxcvbnm"]

alphabet = "abcdefghijklmnopqrstuvwxyz"

pinpad_rows = ["123", "456", "789"]  # first and last row can be interchanged
pinpad_columns = ["147", "258", "369"]

numbers = "01234567890"  # zero can be either before or after the other numbers

special_char_line = "!@#$%^&*()"
special_chars = ".,:;\'\"?!@#$£€%^&(){}[]<>/\\_+-*÷="

capital_discriminative_chars = "abdehmnqrty"  # intuitively + best precision & recall

layout_discriminative_chars = "dgkmquvwx"  # not in special key words (space, shift...)
