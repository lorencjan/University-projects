# File: character_rules.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: This file exports layout rules for characters to be checked, meaning expected characters
#              on the left and right side of a character for layout rows, keywords etc.

import constants as c
import layouts as l


def create_char_rule(key, left_neighbors, right_neighbors):
    """ Helper function for creation of an object representing a character rule.

        Parameters
        ----------
        key : str
            Name of the keyboard key (character or a keyword)
        left_neighbors : str
            Expected neighboring characters on the left as a string
        right_neighbors : str
            Expected neighboring characters on the right as a string

        Returns
        -------
        _ : dict
            Object representing a rule
    """
    
    return {"key": key, "left": left_neighbors, "right": right_neighbors}


def get_char_neighbors(character, sequence):
    """ Splits a sequence to 2 parts by a character, hence obtaining its left|right neighbors.

        Parameters
        ----------
        character : str
            Character whose neighbors we're getting
        sequence : str
            Expected sequence (row, keyword ...) in which the character should be

        Returns
        -------
        _ : tuple(list, list)
            Expected left and right neighbors of a character
    """
    
    idx = sequence.index(character)
    return sequence[:idx], sequence[idx + 1:]


def get_char_neighbors_from_grid(character, grid):
    """ Finds a row in a grid containing a character and splits it to 2 parts by a character,
        hence obtaining its left|right neighbors.

        Parameters
        ----------
        character : str
            Character whose neighbors we're getting
        grid : list(str)
            List of rows (e.g. qwerty layout) containing a rows in which the character should be

        Returns
        -------
        _ : tuple(list, list)
            Expected left and right neighbors of a character
    """

    return get_char_neighbors(character, next(row for row in grid if character in row))


def create_layout_rules(character):
    """ Creates character rules for layouts (qwerty and alphabet are supported).

        Parameters
        ----------
        character : str
            Character for which the rules are created

        Returns
        -------
        _ : dict
            Dictionary with layout rules for the given character
    """

    return {
        c.layout: {
            c.qwerty: create_char_rule(character, *get_char_neighbors_from_grid(character, l.qwerty)),
            c.alphabet: create_char_rule(character, *get_char_neighbors(character, l.alphabet))
        },
        c.special: []
    }


def create_numbers_dict():
    """ Creates character rules for numbers. Rules aims at pinpad layout and ordered number line.

        Returns
        -------
        _ : dict
            Dictionary with rules for numbers
    """

    return {number: {
        c.pinpad: create_char_rule(number, *get_char_neighbors_from_grid(number, l.pinpad_rows)),
        c.pinpad_column: next(col for col in l.pinpad_columns if number in col).replace(number, ""),
        c.number_line: create_char_rule(number, *get_char_neighbors(number, l.numbers)),
        c.special: []
    } for number in "123456789"}


def add_keywords_rules(rules_dict):
    """ Creates character rules for special keys (keywords).

        Parameters
        ----------
        rules_dict : dict
            Dictionary with character rules to which the keyword rules are to be added
    """
    
    # helper local function reducing boilerplate for adding to the dictionary
    def append(character): return rules_dict[character][c.special].append

    # add rules for keywords without duplicates
    for word in [c.space, c.shift, c.tab, c.capslock]:
        for ch in word:
            append(ch)(create_char_rule(word, *get_char_neighbors(ch, word)))

    # space have commonly written keyboard language on it instead of actual 'space' -> support additional word
    for ch in "english":
        append(ch)(create_char_rule(c.space, *get_char_neighbors(ch, "english")))

    # enter have duplicate 'e' inside -> create 2 rules for 'e'
    for ch in c.enter:
        if ch == "e":
            append(ch)(create_char_rule(c.enter, "", "nter")),
            append(ch)(create_char_rule(c.enter, "ent", "r"))
        else:
            append(ch)(create_char_rule(c.enter, *get_char_neighbors(ch, c.enter)))

    # enter have sometimes 'return' (as carriage return) on it instead of actual 'enter' -> support additional word
    for ch in "return":
        append(ch)(create_char_rule(c.enter, *get_char_neighbors(ch, "return")))

    # the 'capslock' constant means just 'caps'
    # for ch in "capslock":
    #     if ch == "c":
    #         append(ch)(create_char_rule(c.capslock, "", "apslock")),
    #         append(ch)(create_char_rule(c.capslock, "capslo", "k"))
    #     else:
    #         append(ch)(create_char_rule(c.capslock, *get_char_neighbors(ch, c.capslock)))

    # backspace have duplicate 'a' and 'c' inside -> create 2 rules for those
    for ch in c.backspace:
        if ch == "a":
            append(ch)(create_char_rule(c.backspace, "b", "ckspace")),
            append(ch)(create_char_rule(c.backspace, "backsp", "ce"))
        elif ch == "c":
            append(ch)(create_char_rule(c.backspace, "ba", "kspace")),
            append(ch)(create_char_rule(c.backspace, "backspa", "e"))
        else:
            append(ch)(create_char_rule(c.backspace, *get_char_neighbors(ch, c.backspace)))

    # backspace have sometimes shorter variations on it instead of actual 'backspace' -> support additional words
    for ch in "backsp":
        append(ch)(create_char_rule(c.backspace, *get_char_neighbors(ch, "backsp")))
    for ch in "bksp":
        append(ch)(create_char_rule(c.backspace, *get_char_neighbors(ch, "bksp")))

    # add mode character sequences
    for mode_variant in c.modes:
        for ch in mode_variant:
            if ch not in rules_dict.keys():  # can be a special character -> not yet in rules dict
                rules_dict[ch] = {c.special: []}

            append(ch)(create_char_rule(c.mode, *get_char_neighbors(ch, mode_variant)))

    # support for paging
    if "/" not in rules_dict.keys():       # just check '/' as e.g. finding 1,2 next to each other
        rules_dict["/"] = {c.special: []}  # does not need to mean 1/2 with unrecognized '/'

    for page_variant in c.pages:
        append("/")(create_char_rule(c.page, *get_char_neighbors("/", page_variant)))


def add_special_char_rules(rules_dict):
    """ Creates character rules for special characters
        (basically just common character line which can be found e.g. above qwerty row).

        Parameters
        ----------
        rules_dict : dict
            Dictionary with character rules to which the special character rules are to be added
    """

    for ch in l.special_char_line:
        rule = create_char_rule(ch, *get_char_neighbors(ch, l.special_char_line))
        if ch not in rules_dict.keys():
            rules_dict[ch] = {c.char_line: rule}
        else:
            rules_dict[ch][c.char_line] = rule


# actual creation of the rules
char_rules = {ch: create_layout_rules(ch) for ch in l.alphabet}
char_rules.update(create_numbers_dict())
add_keywords_rules(char_rules)
add_special_char_rules(char_rules)
