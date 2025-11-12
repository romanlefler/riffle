import "./styles/dialog.css";

export interface DialogOptions {
    content : string;
    title? : string | undefined;
    buttons? : string[] | undefined;
}

export const DIALOG_OK = [ "OK" ];
export const DIALOG_YES_NO = [ "No", "Yes" ];

export async function showDialog(options : DialogOptions) : Promise<number> {
    
    const back = document.createElement("div");
    back.classList.add("dialog-back");

    const box = document.createElement("div");
    box.classList.add("dialog-box");
    back.appendChild(box);

    const flex = document.createElement("div");
    flex.classList.add("dialog-flex");
    box.appendChild(flex);

    const title = document.createElement("div");
    title.classList.add("dialog-title");
    title.textContent = options.title ?? "Attention";
    flex.appendChild(title);

    const content = document.createElement("div");
    content.classList.add("dialog-content");
    content.textContent = options.content;
    flex.appendChild(content);

    const buttonsRow = document.createElement("div");
    buttonsRow.classList.add("dialog-buttons-row");
    flex.appendChild(buttonsRow);

    let data = options.buttons;
    if(!data || data.length < 1) data = DIALOG_OK;

    let btn : HTMLButtonElement;
    const promise : Promise<number> = new Promise(resolve => {
        for(let i = 0; i < data.length; i++) {
            btn = document.createElement("button");
            btn.classList.add("dialog-button");
            btn.textContent = data[i];
            btn.addEventListener("click", () => {
                back.remove();
                resolve(i);
            });
            buttonsRow.appendChild(btn);
        }

        document.querySelector("body main")!.appendChild(back);
        btn!.focus();
    });

    return promise;
}
