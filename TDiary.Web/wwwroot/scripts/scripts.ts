namespace TDiaryInterop {

    class TDiaryFunctions {
        public isOnline(): boolean {
            const result = navigator.onLine;
            return result;
        }
    }

    export function Load(): void {
        window['tdiaryFunctions'] = new TDiaryFunctions();
    }
}

TDiaryInterop.Load();