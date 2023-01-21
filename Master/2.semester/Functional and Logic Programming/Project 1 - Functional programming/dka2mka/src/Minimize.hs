-- Solution: FLP - project 1 - dka-2-mka
-- File: Minimize.hs
-- Author: Jan Lorenc
-- Login: xloren15
-- University: Brno University of technology: Faculty of information technology
-- Date: 21.2.2022

module Minimize ( minimizeFA ) where

import Types
import Data.Maybe ( fromMaybe )
import qualified Data.Set as Set
import qualified Data.Map as Map

-- splits states on the initial two classes - 'final states' and 'not final states'
initEquivalenceClasses :: FA -> EquivalenceClasses
initEquivalenceClasses fa = Set.fromList $ (\(x, y) -> [x, y]) $ Set.partition (`Set.member` finalStates fa) (states fa)

-- checks whether two states are in the same class
areStatesInTheSameClass :: State -> State -> StateClasses -> FA -> Bool
areStatesInTheSameClass s1 s2 cs fa = do
    let findDestination = \s x -> Map.lookup x $ Map.findWithDefault Map.empty s (rules fa)
    -- checks for both states if their destinations for given symbol share the same class
    let hasSameDestination :: Symbol -> Bool
        hasSameDestination x = case (findDestination s1 x, findDestination s2 x) of
            (Nothing, Nothing) -> True                                          -- neither state has a rule for the symbol
            (Just dst1, Just dst2) -> Map.lookup dst1 cs == Map.lookup dst2 cs  -- are destinations in the same classes?
            _ -> False

    all hasSameDestination (Set.toList $ alphabet fa)

-- helper accumulating function that adds state to appropriate class
putStateToClass :: FA -> StateClasses -> [EquivalenceClass] -> State -> [EquivalenceClass]
putStateToClass fa scs cs s = do
    let predicate = (\c -> areStatesInTheSameClass s (head $ Set.elems c) scs fa)
    let newClass = Set.fromList [s]

    let insertOrUpdate :: [EquivalenceClass] -> [EquivalenceClass]
        insertOrUpdate [] = [newClass]
        insertOrUpdate (c:cs') = if predicate c then Set.insert s c : cs' else c : insertOrUpdate cs'

    insertOrUpdate cs

-- finds to which class belongs each state
equivalenceClasses2StateClasses :: EquivalenceClasses -> StateClasses
equivalenceClasses2StateClasses = Set.foldl addStates Map.empty
    where
        -- updates the map for states of a class (s1:class, s2:class ...)
        addStates :: StateClasses -> States -> StateClasses
        addStates scs ss = do
            let statesOfOneClass = Map.fromList [ (s, ss) | s <- Set.toList ss ]
            Map.union scs statesOfOneClass

-- recursively performs the class splitting until there's no need for any further split
splitEquivalenceClasses :: FA -> EquivalenceClasses -> StateClasses
splitEquivalenceClasses fa cs = do
    let stateClasses = equivalenceClasses2StateClasses cs
    let putStateToClassPartial = putStateToClass fa stateClasses

    -- splits one class from set of equivalence classes
    let executeSplit :: EquivalenceClasses -> States -> EquivalenceClasses
        executeSplit classes statesClass = Set.union classes splitClass
            where splitClass = Set.fromList $ Set.foldl putStateToClassPartial [] statesClass

    let newEquivalenceClasses = Set.foldl executeSplit Set.empty cs
    if cs == newEquivalenceClasses  -- check the fix-point
        then stateClasses
        else splitEquivalenceClasses fa newEquivalenceClasses

-- the classes represent new states -> this function assings states = numbers to the classes
classes2States :: FA -> StateClasses -> Map.Map EquivalenceClass State -> [State] -> State -> Map.Map EquivalenceClass State
classes2States _ _ acc [] _ = acc
classes2States fa scs acc (s:ss) newState = do
    let currClass = getEquivalenceClass s scs
    if Map.member currClass acc then classes2States fa scs acc ss newState else do
        let destinations = Map.elems $ getRuleSteps s (rules fa)
        let accUpdated = Map.insert currClass newState acc
        classes2States fa scs accUpdated (ss ++ destinations) (newState + 1)

-- transforms an FA to a complete FA by adding a sink state and appropriate rules
appendSinkState :: FA -> FA
appendSinkState fa = do
    let faAlphabet = alphabet fa
    let faStates = states fa
    let faRules = rules fa
    let sinkState = 1 + Set.findMax faStates  -- it's a new state, so the next number
    let ruleStepsToSink = Map.fromList [ (symbol, sinkState) | symbol <- Set.toList faAlphabet ]
    let ruleLessStates = Set.difference faStates $ Map.keysSet faRules

    -- adds the sink state and completes rules
    let updateFA :: StateRules -> FA
        updateFA srs = do
            let completeRules = Map.map (`Map.union` ruleStepsToSink) srs  -- add steps to sink for all missing rules to all states
            fa {
                states = Set.insert sinkState faStates,
                rules = Map.insert sinkState ruleStepsToSink completeRules -- add all steps to sink to the sink state
            }

    if Set.null ruleLessStates
        then do -- all states have rules -> if the FA is complete, do nothing, otherwise complete with sink
            if all (\rss -> Map.keysSet rss == faAlphabet) (Map.elems faRules)
                then fa
                else updateFA faRules
        else do -- states without rules will get all steps to sink
            let f = \acc s -> Map.insert s ruleStepsToSink acc
            let newRules = Set.foldl f faRules ruleLessStates
            updateFA newRules

-- removes unreachable states and their rules
removeUnreachableStates :: FA -> FA
removeUnreachableStates fa = do    
    let addReachableStates = \ss s -> Set.union ss $ Set.fromList $ Map.elems $ getRuleSteps s (rules fa)
    let makeStep = \ss -> Set.foldl addReachableStates ss ss
    let shouldEnd = \ss -> ss == makeStep ss
    let reachableStates = until shouldEnd makeStep $ Set.fromList [initState fa]
    let unreachableStates = Set.difference (states fa) reachableStates
    fa {
        states = reachableStates,
        finalStates = Set.intersection reachableStates (finalStates fa),
        rules = Map.filterWithKey (\s _ -> Set.notMember s unreachableStates) (rules fa)
    }

-- performs minimization of a deterministic finite automate
minimizeFA :: FA -> FA
minimizeFA fa = do
    -- get FA which has no unreachable states and is complete
    let completeFA = appendSinkState $ removeUnreachableStates fa

    -- compute new minimized states
    let equivalenceClasses = initEquivalenceClasses completeFA
    let stateClasses = splitEquivalenceClasses completeFA equivalenceClasses
    let newStates = classes2States completeFA stateClasses Map.empty [initState completeFA] 0

    -- helper lambdas transforming original states and rules to the new ones
    let originalState2NewClassState = \s -> fromMaybe 0 $ Map.lookup (getEquivalenceClass s stateClasses) newStates
    let originalRules2NewClassRules = \(s, rss) -> (originalState2NewClassState s, Map.map originalState2NewClassState rss)

    completeFA {
       states = Set.fromList $ Map.elems newStates,
       initState = originalState2NewClassState (initState completeFA),
       finalStates = Set.map originalState2NewClassState (finalStates completeFA),
       rules = Map.fromList $ map originalRules2NewClassRules $ Map.assocs (rules completeFA)
    }
