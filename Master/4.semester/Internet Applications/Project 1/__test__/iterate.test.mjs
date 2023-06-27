/**
 * @file
 * <p>File: iterate.test.mjs</p>
 * <p>Description: Jest unit tests for iterateProperties generator from iterare.mjs module.
 *                 The tests focus on edge cases, assignment functionality examples and custom scenarios.
 * </p>
 * <p>Course: WAP - Internet applications</p>
 * <p>Assignment: Prototype chain traversal</p>
 * <p>University: Brno University of Technology - Faculty of Information Technology</p>
 * <p>Date: 12.02.2023</p>
 * 
 * @author
 * Jan Lorenc (xloren15)
 */

import { iterateProperties } from "../iterate.mjs";

/**
 * Helper function getting iterator results as an array
 * @param {any} obj Object which properties the iterator should get
 * @param {object} filter Optional filter descriptor object
 * @returns {string[]} All generator values in an array
 */
const getProps = (obj, filter) => [...iterateProperties(obj, filter)];

/** Property names of Object.prototype - ground truth for the tests */
const prototypeProps = ["constructor", "__defineGetter__", "__defineSetter__", "hasOwnProperty",
                        "__lookupGetter__", "__lookupSetter__", "isPrototypeOf", "propertyIsEnumerable",
                        "toString", "valueOf", "__proto__", "toLocaleString"];

/***** Tests for assignment examples validity  *****/

let o = { a: 1, b: 2, c: 3 };
let p = Object.create(o);
Object.defineProperty(p, "a", {
	value: 10,
	writable: false,
	enumerable: true,
});

const oKeys = Object.keys(o);
const pKeys = Object.keys(p);

describe("Assignment examples", () => {

    test("Example 1", () => {
        const props = getProps(Object.prototype);

        expect(props).toHaveLength(prototypeProps.length);
        expect(props).toEqual(expect.arrayContaining(prototypeProps));
    });

    test("Example 2", () => {
        const props = getProps(o);

        expect(props).toHaveLength(prototypeProps.length + oKeys.length);
        expect(props).toEqual(expect.arrayContaining([...prototypeProps, ...oKeys]));
    });

    test("Example 3", () => {
        const props = getProps(p);

        expect(props).toHaveLength(prototypeProps.length + oKeys.length + pKeys.length);
        expect(props).toEqual(expect.arrayContaining([...prototypeProps, ...oKeys, ...pKeys]));
    });

    test("Example 4", () => {
        const props = getProps(p, {enumerable: true});

        expect(props).toHaveLength(oKeys.length + pKeys.length);
        console.log([...oKeys, ...pKeys])
        expect(props).toEqual(expect.arrayContaining([...oKeys, ...pKeys]));
    });

    test("Example 5", () => {
        const props = getProps(p, {writable: false});

        expect(props).toHaveLength(pKeys.length);
        expect(props).toEqual(expect.arrayContaining([...pKeys]));
    });

    test("Example 6", () => {
        let result = [];
        let it1 = iterateProperties(p, {enumerable: true});
        result.push(it1.next().value); // a
        result.push(it1.next().value); // b
        let it2 = iterateProperties(p, {enumerable: true});
        result.push(it2.next().value); // a
        result.push(it1.next().value); // c
        result.push(it2.next().value); // b
        result.push(it2.next().value); // c
        
        expect(result).toEqual(["a", "b", "a", "c", "b", "c"]);
    });

    test("Example 7", () => {
        function Point(x, y) {
            this.x = x;
            this.y = y;
        }
        Point.prototype = {
            x: undefined,
            y: undefined
        }

        const props = getProps(new Point(1.5, 6.3), {enumerable: true});

        expect(props).toEqual(["x", "y", "x", "y"]);
    });

    test("Example 8", () => {
        class Point3D {
            x = undefined;
            y = undefined;
            z = undefined;
            navíc = undefined;
            constructor(x,y,z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        const props = getProps(new Point3D(1.5, 6.3, 5), {enumerable: true});

        expect(props).toEqual(["x", "y", "z", "navíc"]);
    });
});

/***** Unit tests for custom scenarios  *****/

describe("Edge cases", () => {

    test("Generator shouldn't throw on null object and instead it should be empty", () => {
        const result = iterateProperties(null).next();

        expect(result.value).toBe(undefined);
        expect(result.done).toBe(true);
    });

    test("Generator shouldn't throw on undefined and instead it should be empty", () => {
        const result = iterateProperties(undefined).next();

        expect(result.value).toBe(undefined);
        expect(result.done).toBe(true);
    });

    test("Generator is capable of handling non-object types (string, number, boolean, function)", () => {
        expect(getProps("")).toHaveLength(61);        // basic string has 61 props
        expect(getProps("abc")).toHaveLength(64);     // and additional one for each character
        expect(getProps(0)).toHaveLength(19);         // number
        expect(getProps(true)).toHaveLength(15);      // boolean
        expect(getProps(() => {})).toHaveLength(23);  // function
    });

    test("Generator shouldn't throw on non-object filter and it should treat it as no filter", () => {
        const filterValues = [null, undefined, 5, "abc", true, () => {}];
        for(let filter of filterValues) {
            const props = getProps(Object.prototype, filter);

            expect(props).toHaveLength(prototypeProps.length);
            expect(props).toEqual(expect.arrayContaining(prototypeProps))
        }
    });

    test("Nothing should match a made up descriptor property as the comparison is supposed to be 'strict'", () => {
        const props = getProps(Object.prototype, { madeUpProp: true });

        expect(props).toHaveLength(0);
        expect(props).toEqual([])
    });
});


describe("Functionality - basic", () => {
    // basic functionality is already tested by provided examples -> adding more nested inheritance example
    test("Generator finds all properties in long inheritance chain", () => {
        const obj1 = { a: undefined, b: undefined };
        const obj2 = Object.create(obj1, {c: { value: 0 }, d: { value: null } });
        const obj3 = Object.create(obj2, {a: { value: 0 }, b: { value: null } });
        const obj4 = Object.create(obj3, {x: { value: 0 }, y: { value: null } });

        const props = getProps(obj4);

        expect(props).toHaveLength(prototypeProps.length + 8);
        expect(props).toEqual(expect.arrayContaining([...prototypeProps, "a", "b", "c", "d", "x", "y"]));
        expect(props.filter(x => x === "a")).toHaveLength(2);
        expect(props.filter(x => x === "b")).toHaveLength(2);
    });
});

describe("Functionality - filters", () => {

    test("Filters by value in descriptor", () => {
        const obj = { x: 10 };

        const props = getProps(obj, { value: 10 });

        expect(props).toHaveLength(1);
        expect(props).toEqual(["x"]);
    });

    test("Missing descriptor property means filter is not applied on it", () => {
        const descriptor = {
            writable: false,
            configurable: false,
            value: 0  // not to need checking all descriptor values of all Object.prototype props
        };
        const obj = Object.create(Object.prototype, { 
            x: { ...descriptor, enumerable: true },
            y: { ...descriptor, enumerable: false }
        });

        const props = getProps(obj, descriptor);

        expect(props).toHaveLength(2);
        expect(props).toEqual(["x", "y"]);
    });

    test.each`
      enumerable | writable | configurable
       ${true}   | ${true}  |   ${true}
       ${true}   | ${true}  |   ${false}
       ${true}   | ${false} |   ${true}
       ${false}  | ${true}  |   ${true}
       ${false}  | ${false} |   ${true}
       ${false}  | ${true}  |   ${false}       
       ${true}   | ${false} |   ${false}
       ${false}  | ${false} |   ${false}
  `("Filters correctly on each combination of basic descriptor properties", ({enumerable, writable, configurable}) => {
        const descriptor = { 
            enumerable: enumerable,
            writable: writable,
            configurable: configurable,
            value: 0  // not to need checking all descriptor values of all Object.prototype props
        };
        const obj = Object.create(Object.prototype, { x: descriptor });

        const props = getProps(obj, descriptor);

        expect(props).toHaveLength(1);
        expect(props).toEqual(["x"]);
    });
});