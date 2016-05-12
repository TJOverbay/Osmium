"use strict";
var SimpleClass = (function () {
    function SimpleClass() {
        this.name = "Barney";
    }
    Object.defineProperty(SimpleClass.prototype, "message", {
        get: function () {
            return "Hello ES2015!";
        },
        enumerable: true,
        configurable: true
    });
    SimpleClass.prototype.calculate = function () {
        return 42;
    };
    return SimpleClass;
}());
exports.SimpleClass = SimpleClass;
