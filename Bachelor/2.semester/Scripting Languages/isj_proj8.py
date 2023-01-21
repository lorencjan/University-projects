#!/usr/bin/env python3

def first_with_given_key(iterable, key=None):
    '''function returns iterably either all members of 
       given iterable or reduced by key function'''
    used = list()
    for member in iterable:
        if key is None:
            yield member
        else:
            keyResult = key(member)
            if keyResult not in used:
                yield member
            used.append(keyResult)
