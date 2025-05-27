
export async function sleep(seconds : number) : Promise<void> {
    return new Promise<void>(resolve => {
        setTimeout(resolve, seconds * 1000.0);
    });
}
