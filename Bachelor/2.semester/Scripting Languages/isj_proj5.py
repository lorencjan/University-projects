#!/usr/bin/env python3

def gen_quiz (qpool, *indexes, altcodes='ABCDEF', quiz=None):
    """>>> test_qpool1 = [('Question1', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question2', ['Answer1', 'Answer2', 'Answer3']), ('Question3', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question4', ['Answer1', 'Answer2'])]
    >>> existing_quiz1 = [('ExistingQuestion1', ['1: Answer1', '2: Answer2', '3: Answer3']), ('ExistingQuestion2', ['1: Answer1', '2: Answer2'])]
    >>> gen_quiz(test_qpool1, -2, 0, altcodes = ('10', '20', '30'), quiz = existing_quiz1)
    [('ExistingQuestion1', ['1: Answer1', '2: Answer2', '3: Answer3']), ('ExistingQuestion2', ['1: Answer1', '2: Answer2']), ('Question3', ['10: Answer1', '20: Answer2', '30: Answer3']), ('Question1', ['10: Answer1', '20: Answer2', '30: Answer3'])]

    >>> test_qpool2 = [('Question1', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question2', ['Answer1', 'Answer2', 'Answer3']), ('Question3', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question4', ['Answer1', 'Answer2'])]
    >>> existing_quiz2 = [('ExistingQuestion1', ['1: Answer1', '2: Answer2', '3: Answer3']), ('ExistingQuestion2', ['1: Answer1', '2: Answer2'])]
    >>> not_used = gen_quiz(test_qpool2, -2, 0, altcodes = ('1', '2', '3'), quiz = existing_quiz2)
    >>> existing_quiz2
    [('ExistingQuestion1', ['1: Answer1', '2: Answer2', '3: Answer3']), ('ExistingQuestion2', ['1: Answer1', '2: Answer2']), ('Question3', ['1: Answer1', '2: Answer2', '3: Answer3']), ('Question1', ['1: Answer1', '2: Answer2', '3: Answer3'])]

    >>> test_qpool3 = [('Question1', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question2', ['Answer1', 'Answer2', 'Answer3']), ('Question3', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question4', ['Answer1', 'Answer2'])]
    >>> existing_quiz3 = [('ExistingQuestion1', ['1: Answer1', '2: Answer2', '3: Answer3']), ('ExistingQuestion2', ['1: Answer1', '2: Answer2'])]
    >>> not_used1 = gen_quiz(test_qpool3, 0, 2, altcodes = ('i', 'ii', 'iii'), quiz = existing_quiz3)
    >>> not_used2 = gen_quiz(test_qpool3, 1, altcodes = ('i', 'ii', 'iii'), quiz = existing_quiz3)
    >>> existing_quiz3
    [('ExistingQuestion1', ['1: Answer1', '2: Answer2', '3: Answer3']), ('ExistingQuestion2', ['1: Answer1', '2: Answer2']), ('Question1', ['i: Answer1', 'ii: Answer2', 'iii: Answer3']), ('Question3', ['i: Answer1', 'ii: Answer2', 'iii: Answer3']), ('Question2', ['i: Answer1', 'ii: Answer2', 'iii: Answer3'])]

    >>> test_qpool4 = [('Question1', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question2', ['Answer1', 'Answer2', 'Answer3']), ('Question3', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question4', ['Answer1', 'Answer2'])]
    >>> gen_quiz(test_qpool4, 0, 1, -1)
    [('Question1', ['A: Answer1', 'B: Answer2', 'C: Answer3', 'D: Answer4']), ('Question2', ['A: Answer1', 'B: Answer2', 'C: Answer3']), ('Question4', ['A: Answer1', 'B: Answer2'])]

    >>> test_qpool5 = [('Question1', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question2', ['Answer1', 'Answer2', 'Answer3']), ('Question3', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question4', ['Answer1', 'Answer2'])]
    >>> not_used = gen_quiz(test_qpool5, 0, altcodes = '123456')
    >>> gen_quiz(test_qpool5, 0, altcodes = '123456')
    [('Question1', ['1: Answer1', '2: Answer2', '3: Answer3', '4: Answer4'])]
    
    >>> test_qpool6 = [('Question1', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question2', ['Answer1', 'Answer2', 'Answer3']), ('Question3', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question4', ['Answer1', 'Answer2'])]
    >>> existing_quiz6 = [('ExistingQuestion1', ['1: Answer1', '2: Answer2', '3: Answer3']), ('ExistingQuestion2', ['1: Answer1', '2: Answer2'])]
    >>> gen_quiz(test_qpool6, quiz = existing_quiz6)
    [('ExistingQuestion1', ['1: Answer1', '2: Answer2', '3: Answer3']), ('ExistingQuestion2', ['1: Answer1', '2: Answer2'])]

    >>> test_qpool7 = [('Question1', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question2', ['Answer1', 'Answer2', 'Answer3']), ('Question3', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question4', ['Answer1', 'Answer2'])]
    >>> gen_quiz(test_qpool7, 0, 4, 2, altcodes = ['101','201']) 
    Ignoring index 4 - list index out of range
    [('Question1', ['101: Answer1', '201: Answer2']), ('Question3', ['101: Answer1', '201: Answer2'])]

    >>> test_qpool8 = [('Question1', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question2', ['Answer1', 'Answer2', 'Answer3']), ('Question3', ['Answer1', 'Answer2', 'Answer3', 'Answer4']), ('Question4', ['Answer1', 'Answer2'])]
    >>> gen_quiz(test_qpool8, 1, 0, 3, 2, altcodes = ['1', '2', '3', 4]) 
    Ignoring index 0 - unsupported operand type(s) for +: 'int' and 'str'
    Ignoring index 2 - unsupported operand type(s) for +: 'int' and 'str'
    [('Question2', ['1: Answer1', '2: Answer2', '3: Answer3']), ('Question4', ['1: Answer1', '2: Answer2'])]
    """
    if quiz is None:
        quiz = []
    result = quiz
    for index in indexes:
        try:
            counter=0
            question = (qpool[index][0], [])
            for answer in qpool[index][1]:
                if counter < len(altcodes):
                    question[1].append(altcodes[counter] + ": " + answer)
                counter += 1
            result.append(question)
        except IndexError as e1:
            print("Ignoring index {0} - {1}".format(index, e1))
        except TypeError as e2:
            print("Ignoring index {0} - {1}".format(index, e2))
    return result

if __name__ == "__main__":
    import doctest
    doctest.testmod()