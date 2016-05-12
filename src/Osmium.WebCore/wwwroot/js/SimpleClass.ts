export class SimpleClass {
    constructor() {
        this.name = "Barney";
    }

    name: string;

    get message(): string {
        return "Hello ES2015!";
    }

    calculate(): number {
        return 42;
    }
}