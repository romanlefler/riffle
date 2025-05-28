
export class RedirectInterruptError extends Error {

    constructor() {
        super("The browser is redirecting away from here.");
        this.name = "RedirectError";
    }

}