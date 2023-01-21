#!/usr/bin/env python3

class Polynomial:
    ''' Class represents a polynomial and allows mathematical operations such as
        addition, subtraction, multiplication, derivation, powering, comparison
        or getting the value of the polynomial in specific value as x over given polynomial '''
    def __init__(self, *args, **kwargs):
        ''' turns input into list and from lowest exponent to highest '''
        #if a list o tuple was give get it and convert it to list
        if args:
            if type(args[0]) is list:
                self.variables = args[0]
            else:
                self.variables = list(args)
        #get list from keyword arguments
        else:
            self.variables = list()
            highestExp = 0
            for key in sorted(kwargs):
                if highestExp < int(key[1:]):
                    highestExp = int(key[1:])
            counter = 0
            while counter <= highestExp:
                if "x{0}".format(counter) in kwargs.keys():
                    self.variables.append(kwargs["x{0}".format(counter)])
                else:
                    self.variables.append(0)
                counter+=1
        #if the highest members are zero, remove them
        counter = len(self.variables)-1
        while counter >= 0:
            if self.variables[counter] == 0:
                del self.variables[counter]
            else:
                break
            counter-=1
    
    def __eq__(self, other):
        ''' method compares 2 polynomials '''
        return self.variables == other.variables
    
    def __add__(self, other):
        ''' method subtract one polynomial from another '''
        result = list()
        counter = 0
        while counter < len(self.variables):
            if len(other.variables) <= counter:
                result.append(self.variables[counter])
            else:
                result.append(self.variables[counter]+other.variables[counter])
            counter+=1
        #if second is larger than the first, get the rest
        while counter < len(other.variables):
            result.append(other.variables[counter])
            counter+=1
        return Polynomial(result)
    
    def __sub__(self, other):
        ''' method subtract one polynomial from another '''
        result = list()
        counter = 0
        while counter < len(self.variables):
            if len(other.variables) <= counter:
                result.append(self.variables[counter])
            else:
                result.append(self.variables[counter]-other.variables[counter])
            counter+=1
        #if second is larger than the first, get the rest
        while counter < len(other.variables):
            result.append(-other.variables[counter])
            counter+=1
        return Polynomial(result)
    
    def __mul__(self, other):
        ''' multiply one polynom by another '''
        pol1 = self.variables
        pol2 = other.variables
        #algorithm for polynomial multiplication
        tmp = [ [0]*o2+[i1*i2 for i1 in pol1]+[0]*(len(pol1)-o2) for o2,i2 in enumerate(pol2)]
        length = len(pol1)+len(pol2)-1
        return Polynomial([ sum(row[i] for row in tmp) for i in range(length)])
    
    def __pow__(self, exp):
        ''' powers itself by given exponent '''
        counter = 1
        tmp = Polynomial(self.variables)
        while counter < exp:
            tmp *= Polynomial(self.variables)
            counter += 1
        return tmp
    
    def derivative(self):
        ''' derives itself '''
        derivedMembers = list()
        exp = 1
        for i in range(1, len(self.variables)):
            derivedMembers.append(self.variables[i] * exp)
            exp += 1
        return Polynomial(derivedMembers)
    
    def at_value(self, *values):
        ''' calculates value of polynomial at given x '''
        #if only one value is given, calculates polynomial value
        if len(values) == 1:
            result = self.variables[0]
            exp = 1
            for member in self.variables[1:]:
                result += values[0]**exp * member
                exp+=1
            return result
        #if 2 values are given, calculates difference in the polynomial values
        elif len(values) == 2:
            results = list()
            counter = 0
            while counter < 2:
                result = self.variables[0]
                exp = 1
                for member in self.variables[1:]:
                    result += values[counter]**exp * member
                    exp+=1
                results.append(result)
                counter+=1
            return results[1]-results[0]
        else:
            return "Error: Function takes only 1 or 2 values!"
        
    
    def __str__(self):
        ''' prints class in polynomial style '''
        toPrint = ""
        counter = 0
        #creating string of polynomial members
        for var in self.variables:
            if var != 0:
                if counter == 0:
                    toPrint += str(var)
                elif counter == 1:
                    toPrint = "{0}x".format(var) + " + " + toPrint
                else:
                    toPrint = "{0}x^{1}".format(var, counter) + " + " + toPrint
            counter+=1
        if toPrint == "":
            return "0"
        toPrint = toPrint.replace('+ -', '- ') # changes negatives to minus
        if toPrint[-3:] == " + ":              # in case of zero value of last member, superfluoues
            toPrint = toPrint[:-3]             # "+" can be at the end of the string
        return toPrint.replace('1x', 'x')   