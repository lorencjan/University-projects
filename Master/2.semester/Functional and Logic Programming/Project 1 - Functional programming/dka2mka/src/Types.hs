-- Solution: FLP - project 1 - dka-2-mka
-- File: Types.hs
-- Author: Jan Lorenc
-- Login: xloren15
-- University: Brno University of technology: Faculty of information technology
-- Date: 21.2.2022

module Types where

import Data.Set as Set
import Data.List as List
import Data.Map as Map
import Data.Maybe ( fromMaybe )
import Text.Printf ( printf )

-- *** Finite automata ***

-- types for automata members
type State = Int                            -- represents individual state (param convention: s)
type States = Set.Set State                 -- set of states (ss)
type Symbol = Char                          -- represents individual alphabet input symbol (x)
type Alphabet = Set.Set Symbol              -- input symbol alphabet (a)
type Rule = (State, Symbol, State)          -- single rule as original state + input symbol -> new state (r)
type Rules = Set.Set Rule                   -- set of rules (rs)
type RuleStep = (Symbol, State)             -- a step that can be done (from a specific state) (rs)
type RuleSteps = Map.Map Symbol State       -- steps in the form of a map - where we get destinations by a symbol (rss)
type StateRules = Map.Map State RuleSteps   -- steps in accordance with rules that can be applied from each state (srs)

-- finite automata data type
data FA = FA {
    states :: States,
    alphabet :: Alphabet,
    initState :: State,
    finalStates :: States,
    rules :: StateRules
}


-- *** Minimization ***

type EquivalenceClass = States                       -- class is basically just group of states (c)
type EquivalenceClasses = Set.Set States             -- set of classes (cs)
type StateClasses = Map.Map State EquivalenceClass   -- defines to which class certain state belongs (scs)


-- *** Helper methods for working with the types ***

-- looks up steps that can be done from a given state
getRuleSteps :: State -> StateRules -> RuleSteps
getRuleSteps s srs = fromMaybe Map.empty $ Map.lookup s srs

-- looks up an equivalence class of a state from Map of state-class keyvalues
getEquivalenceClass :: State -> StateClasses -> EquivalenceClass
getEquivalenceClass s scs = fromMaybe Set.empty $ Map.lookup s scs

-- extracts list individual rules from more complex StateRules data type
flattenStateRules :: StateRules -> [Rule]
flattenStateRules srs = concat [ ruleStepsToRules state steps | (state, steps) <- Map.toList srs ]
    where
        ruleStepsToRules :: State -> RuleSteps -> [Rule]
        ruleStepsToRules src steps = [ (src, s, dst) | (s, dst) <- Map.toList steps ]

-- displays a finite automata in task output format
printFA :: FA -> IO ()
printFA (FA ss a s0 fs srs) =
    printf "%s\n%s\n%d\n%s\n%s\n" (showStates ss) (showAphabet a) s0 (showStates fs) (showRules $ flattenStateRules srs)
    where
        showStates = List.intercalate "," . fmap show . List.sort . Set.toList
        showAphabet = List.sort . Set.toList
        showRules = \rs -> List.intercalate "\n" [ show src ++ "," ++ [x] ++ "," ++ show dst | (src, x, dst) <- rs ]
