
function NetworkMessage(ID) {
    var me = this;
    var id = ID;
    var parameters = [];

    this.GetID = function () { return id; }
    this.SetParameter = function (name, value) {
        for (var i = 0; i < parameters.length; i++) {
            var pair = parameters[i];
            if (pair[0] == name) {
                pair[1] = value;
                return;
            }
        }

        parameters.push([name, value]);
    }
    this.GetParameter = function (name) {
        for (var i = 0; i < parameters.length; i++) {
            var pair = parameters[i];
            if (pair[0] == name)
                return pair[1];
        }
        return null;
    }
    this.ToText = function () {
        var res = "";

        res += "<BOM>";
        
        res += "ID=" + id + ";";
        for (var i = 0; i < parameters.length; i++) {
            var pair = parameters[i];
            res += pair[0] + "=" + pair[1] + ";";
        }

        res += "<EOM>";

        return res;
    }
    this.FromText = function (txt) {
        txt = txt.substr(5, txt.length - 10); // remove delimiters

        parameters = [];

        var pairs = txt.split(";");
        if (pairs && pairs.length) {
            for (var i = 0; i < pairs.length; i++) {
                var entry = pairs[i];
                if (entry && entry.length) {
                    var pair = entry.split("=");
                    if (pair && pair.length) {
                        if (pair[0] == "ID")
                            id = pair[1];
                        else
                            this.SetParameter(pair[0], pair[1]);
                    } 
                }
            }
        }
    }
}