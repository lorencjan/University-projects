/**
 * @file
 * <p>File: iterate.mjs</p>
 * <p>Description: This module provides iterateProperties generator, which returns property names of a given object.
 *                 Module was developed and tested on node.js version 14.17.0 (corresponding to merlin server at the time).
 * </p>
 * <p>Course: WAP - Internet applications</p>
 * <p>Assignment: Prototype chain traversal</p>
 * <p>University: Brno University of Technology - Faculty of Information Technology</p>
 * <p>Date: 12.02.2023</p>
 * 
 * @author
 * Jan Lorenc (xloren15)
 */

/**
 * Generator which takes any value which properties it should iterate. Ends if the object is not assigned.
 * If it has a parent prototype, it recursively traverses to the parent. It yields all (its own and those of parents) property names.
 * The properties can be optionally filtered by property descriptor values.
 * @param {any} obj Object which properties the iterator should get
 * @param {object} filter Optional filter descriptor object
 * @returns {object} Object with 2 properties - boolean 'done' (if iterator finished) and 'value' (the actual generated value)
 */
export function* iterateProperties(obj, filter = null) {
    if (obj === null || obj === undefined) return;
    obj.__proto__ && (yield* iterateProperties(obj.__proto__, filter));
    for (let name of getPropertyNames(obj, filter))
        yield name;
}

/**
 * Takes an object, filters his properties by optional filter and returns those matching.
 * @param {any} obj Object which property names to get 
 * @param {object} filter Optional filter descriptor object
 * @returns Names of filtered object properties
 */
const getPropertyNames = (obj, filter) =>
    Object
        .entries(Object.getOwnPropertyDescriptors(obj))
        .filter(([_, descriptor]) => !filterDescriptor(filter, descriptor))
        .map(([name, _]) => name);

/**
 * Determines whether a descriptor should be filtered out or kept.
 * @param {object} filter Filter to filter by. If not defined or not a valid object, descriptor is not filtered out
 * @param {object} descriptor Property descriptor under test
 * @returns False (descriptor is kept, not filtered out) if filter is not defined or descriptor matches all filter properties,
 *          otherwise true (some filter property does not match -> filter out)
 */
const filterDescriptor = (filter, descriptor) =>
    filter && typeof filter === "object" && Object.keys(filter).some(x => filter[x] !== descriptor[x]);