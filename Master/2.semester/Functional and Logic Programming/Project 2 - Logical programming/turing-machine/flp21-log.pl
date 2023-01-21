/*
  Solution: FLP Project 2 - Turing Machine
  File: flp21-log.pl
  Author: Jan Lorenc (xloren15)
  University: Brno University of technology: Faculty of information technology
  Date: 04.04.2022
  Description: This projects implements Turing machine simulation in prolog.
*/

% represents a rule
:- dynamic rule/4.

% reads input and stores individual lines into a list
read_lines(Input) :-
    read_line(Line, Char),
	(	
        not(is_eof(Char)), read_lines(Rest), Input = [Line | Rest]
	    ;
        Input = []
	).

% reads a single line
read_line(Line, Char) :-
    get_char(Char),
	(	
        (is_eof(Char) ; is_eol(Char)), Line = [], !
	    ;	
        read_line(T, _), Line = [Char | T]
	).

% checks if input char is EOF
is_eof(Char) :- Char == end_of_file.

% checks if input char is EOL
is_eol(Char) :- char_code(Char, 10).

% converts lines to characters (list of lines -> list of list of chars)
parse_rule_lines([], []).
parse_rule_lines([Line|T], [Chars|TT]) :- parse_rule_lines(T, TT), parse_rule_line(Line, Chars).

% skips spaces between rule characters
parse_rule_line([],[]) :- !.
parse_rule_line([' ',' '|T], [' '|TT]) :- !, parse_rule_line(T,TT). % keep blanks
parse_rule_line([' '|T], TT) :- !, parse_rule_line(T,TT).
parse_rule_line([H|T], [H|TT]) :- parse_rule_line(T,TT).

% prints final output (list of configurations)
write_lines([]).
write_lines([H|T]) :- write_line(H), write_lines(T).

% prints single line
write_line([]) :- write('\n').
write_line([H|T]) :- write(H), write_line(T).

% head function
head([], []).
head([H|_], H).

% opposite to tail (like in haskell) ... gets all but last element
init([_], []):- !.
init([H|T], [H|TT]) :- init(T, TT).

% validates state character
is_state(Char) :- char_type(Char, upper).
is_init_state('S').
is_final_state('F').

% validates symbol character .. either lowercase letter or space (blank)
is_symbol(' ').
is_symbol(Char) :- char_type(Char, lower).

% validates shift character (R or L)
is_right('R').
is_left('L').
is_shift(Char) :- is_right(Char) ; is_left(Char).

% saves rules as lines to the prolog db
save_rules([]).
save_rules([[S1, X1, S2, X2]|T]) :-
    is_state(S1), is_state(S2), is_symbol(X1), (is_symbol(X2);is_shift(X2)), !, 
    assertz(rule(S1, X1, S2, X2)), save_rules(T).

% cleans the db ... to be called at the end of the program
clear_rules :- retractall(rule(_, _, _, _)).

% gets current state
current_state([H|_], H) :- is_state(H).
current_state([_|T], State) :- current_state(T, State).

% gets current symbol under head
current_symbol_under_head([State, Symbol|_], Symbol) :- is_state(State).
current_symbol_under_head([State], ' ') :- is_state(State).
current_symbol_under_head([_|T], Symbol) :- current_symbol_under_head(T, Symbol).

% applies rule to a configuration
applyRule(_, NewSymbol, [State|_], _) :- % cannot go before tape
    is_state(State), is_left(NewSymbol), !, fail.
applyRule(NewState, NewSymbol, [Symbol, State | T], [NewState, Symbol | T]) :- % go left
    is_left(NewSymbol), is_state(State), !.
applyRule(NewState, NewSymbol, [State, Symbol | T], [Symbol, NewState | T]) :- % go right
    is_right(NewSymbol), is_state(State), !.
applyRule(NewState, NewSymbol, [State], [' ', NewState]) :- % go right behind configuration (blank)
    is_right(NewSymbol), is_state(State), !.
applyRule(NewState, NewSymbol, [State, _ | T], [NewState, NewSymbol | T]) :- % rewrite symbol
    is_state(State), !.
applyRule(NewState, NewSymbol, [State], [NewState, NewSymbol]) :- % append symbol
    is_state(State), !.
applyRule(NewState, NewSymbol, [H|T], [H|TT]) :- % nothing? -> continue
    applyRule(NewState, NewSymbol, T, TT).

% executes the Turing machine processing
execute(Config, AllConfigs, Result) :-
    current_state(Config, State), is_final_state(State), reverse([Config|AllConfigs], Result).
execute(Config, AllConfigs, Result) :-
    current_state(Config, State), current_symbol_under_head(Config, Symbol), % where we are
    rule(State, Symbol, NewState, NewSymbol),                                % where we can go
    applyRule(NewState, NewSymbol, Config, NewConfig),                       % go there
    execute(NewConfig, [Config|AllConfigs], Result).                         % recursively continue

% main program
main :-
    prompt(_, ''),
    read_lines(Lines),
    init(Lines, RuleLines),
    last(Lines, InputConfig),
    parse_rule_lines(RuleLines, ParsedRuleLines), !,
    (save_rules(ParsedRuleLines)
      ; write('Input error: Invalid format of the TM.'), clear_rules, fail), !,
    ((head(ParsedRuleLines, FirstRow), head(FirstRow, InitState), is_init_state(InitState)) 
      ; write('Validation error: Initial state must be S.'), clear_rules, fail), !,
    (execute([InitState|InputConfig], [], Configs)
      ; write('Runtime error: Abnormal TM termination.'), clear_rules, fail), !,    
    write_lines(Configs),
    clear_rules, halt.
