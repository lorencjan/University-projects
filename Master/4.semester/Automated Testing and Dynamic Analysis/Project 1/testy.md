# Test documentation  

### CEG
The CEG graph is shown in `CEG_graph.png` and the decision table is saved in `CEG_decision_table.png`. Both are screenshots from the ceg.testos.org program. There are 15 test scenarios:

 * `1-8` - Variations of cart states in which it cannot load any additional material. Those are if there is no more free slots, maximum weight is exceeded or it cannot load more because it is in state of unload only mode
 * `9` - No transport request was made (no matter if it could or could not be accepted).
 * `10` - Brief transition state between creating and accepting a request - on the same path with all other request processing scenarios (basically superfluous)
 * `11` - Transition state between accepting a request and actual loading (travel state) - on the same path with `14` and priority scenarios
 * `12` - Cart couldn't load the material in time, so the material is marked with priority property - on the same path with `15`
 * `13` - Cart couldn't load the priority material in time, so an exception is raised.
 * `14` - Cart was able to load normal material in time.
 * `15` - Cart was able to load priority material in time.
 
 ### Input parameters
 | Name                | Description                         |
 | ------------------- | ----------------------------------- |
 | load_capacity       | maximum cart load capatity          |
 | slots               | number of cart slots                |
 | current_cart_weight | current weight of cart load         |
 | weight              | weight of the material in a request |
 | when                | start time of a request             |
 | path                | src and dst of a request transport  |

 ### Parameter characteristics
 | C_load_capacity | maximum cart load capatity |
 | --------------- | -------------------------- |
 | 1               | 50                         |
 | 2               | 150                        |
 | 3               | 500                        |

 | C_slots | number of cart slots |
 | ------- | -------------------- |
 | 1       | 1                    |
 | 2       | 2                    |
 | 3       | 3                    |
 | 4       | 4                    |

 | C_current_cart_weight | is cart at max weight |
 | --------------------- | --------------------- |
 | 1                     | true                  |
 | 2                     | false                 |

 | C_weight | would current request overload cart |
 | -------- | ----------------------------------- |
 | 1        | true                                |
 | 2        | false                               |

 | C_when | is the request for now or future |
 | ------ | -------------------------------- |
 | 1      | now (when == 0)                  |
 | 2      | future (when > 0)                |

 | C_path | is the path direct                  |
 | ------ | ----------------------------------- |
 | 1      | true (no station in between)        |
 | 2      | false (>= 1 of stations in between) |

 ### Restrictions for combinations of parameter characteristics
 * C_load_capacity.1 -> !C_slots.1
 * C_load_capacity.3 -> !C_slots.3
 * C_load_capacity.3 -> !C_slots.4
 * C_current_cart_weight.1 -> !C_weight.2

 ### Parameter characteristics combinations
 The results of combine.testos.org program for `combine.json` configuration are in screenshot `characteristics_combinations.png`.

 ### Tests
 There are 17 (7 for CEG, 10 for combinations) tests + 1 original happy test. The tables below shows which tests cover which test cases.  
 (Yes, there should be a test for each test case but I merged some very similar test cases because I was already exceeding the 5-15 tests from the assignment specification)  

 #### CEG tests
 | Name                                                                              | Cases covered |
 | --------------------------------------------------------------------------------- | ----------------------------------- |
 | test_exception_is_thrown_if_cart_cannot_handle_request_due_to_unavailable_slots   | `1`, `2`, `3`, `8` - cases 1-8 are just combinations of invalid states leading to same results ... any invalid state causes error -> these cases are when slots cause errors |
 | test_exception_is_thrown_if_cart_cannot_handle_request_due_to_excessive_weight    | `1`, `6`, `7`, `8` - cases 1-8 are just combinations of invalid states leading to same results ... any invalid state causes error -> these cases are when weight causes errors |
 | test_exception_is_thrown_if_cart_cannot_handle_request_being_in_unload_only_state | `1`, `2`, `5`, `6` - cases 1-8 are just combinations of invalid states leading to same results ... any invalid state causes error -> these cases are when state causes errors |
 | test_cart_should_do_nothing_if_no_transport_is_requested                          | `9` |
 | test_successful_load_of_normal_material                                           | `10`, `11`, `14` - these cases have the same path, `10` is just a superfluous state between creating and accepting a request, `11` has the request accepted and waiting and `14` also loads the material |
 | test_successful_load_of_priority_material                                         | `12`, `15` - these cases have the same path, just `15` also loads the priority material while `12` is just the state between accepting and loading |
 | test_exception_is_thrown_if_priority_time_is_exceeded                             | `13` |

#### Combination tests
 | Name                 | Cases covered |
 | -------------------- | ------------- |
 | test_combination_1   | `1`           |
 | test_combination_2   | `2`           |
 | test_combination_3   | `3`           |
 | test_combination_4   | `4`           |
 | test_combination_5   | `5`           |
 | test_combination_6   | `6`           |
 | test_combination_7   | `7`           |
 | test_combination_8   | `8`           |
 | test_combination_9   | `9`           |
 | test_combination_10  | `10`          |