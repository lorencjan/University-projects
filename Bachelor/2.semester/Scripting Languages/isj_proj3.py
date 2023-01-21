#!/usr/bin/env python3

import re

# ukol za 2 body
def first_odd_or_even(numbers):
    evenNum = 0
    oddNum = 0
    firstOdd = ''
    firstEven = ''
    for num in numbers:
        if num % 2:
            evenNum += 1
            if not firstEven:
                firstEven = num
        else:
            oddNum += 1
            if not firstOdd:
                firstOdd = num
    if(evenNum==oddNum or evenNum==0 or oddNum==0):
        return 0
    elif(evenNum>oddNum):
        return firstOdd
    else:
        return firstEven
    """Returns 0 if there is the same number of even numbers and odd numbers
       in the input list of ints, or there are only odd or only even numbers.
       Returns the first odd number in the input list if the list has more even
       numbers.
       Returns the first even number in the input list if the list has more odd 
       numbers.

    >>> first_odd_or_even([2,4,2,3,6])
    3
    >>> first_odd_or_even([3,5,4])
    4
    >>> first_odd_or_even([2,4,3,5])
    0
    >>> first_odd_or_even([2,4])
    0
    >>> first_odd_or_even([3])
    0
    """


# ukol za 3 body
def to_pilot_alpha(word):
    """Returns a list of pilot alpha codes corresponding to the input word

    >>> to_pilot_alpha('Smrz')
    ['Sierra', 'Mike', 'Romeo', 'Zulu']
    """

    pilot_alpha = ['Alfa', 'Bravo', 'Charlie', 'Delta', 'Echo', 'Foxtrot',
        'Golf', 'Hotel', 'India', 'Juliett', 'Kilo', 'Lima', 'Mike',
        'November', 'Oscar', 'Papa', 'Quebec', 'Romeo', 'Sierra', 'Tango',
        'Uniform', 'Victor', 'Whiskey', 'Xray', 'Yankee', 'Zulu']

    pilot_alpha_list = []

    for char in word:
        for word in pilot_alpha:
            if(word[0]==char.upper()):
                pilot_alpha_list.append(word)

    return pilot_alpha_list


if __name__ == "__main__":
    import doctest
    doctest.testmod()
