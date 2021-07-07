var TDiaryInterop;
(function (TDiaryInterop) {
    var TDiaryFunctions = /** @class */ (function () {
        function TDiaryFunctions() {
        }
        TDiaryFunctions.prototype.isOnline = function () {
            var result = navigator.onLine;
            return result;
        };
        return TDiaryFunctions;
    }());
    function Load() {
        window['tdiaryFunctions'] = new TDiaryFunctions();
    }
    TDiaryInterop.Load = Load;
})(TDiaryInterop || (TDiaryInterop = {}));
TDiaryInterop.Load();
//# sourceMappingURL=scripts.js.map