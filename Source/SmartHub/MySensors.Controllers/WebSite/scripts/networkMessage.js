
function NetworkMessage(ID, params) {
    var me = this;
    var id = ID;
    var parameters = params || [];

    this.GetID = function () {
        return id;
    }
    this.GetParameter = function (idx) {
        return parameters[idx];
    }

    this.ToText = function () {
        var res = "";

        res += id;
        for (var i = 0; i < parameters.length; i++)
            res += ";" + parameters[i];
        return res + "\n";
    }
    this.FromText = function (txt) {
        txt = txt.substr(0, txt.length - 1); // remove "\n"
        var parts = txt.split(";");

        if (parts && parts.length) {
            id = parts[0];

            parameters = [];
            for (var i = 1; i < parts.length; i++)
                parameters.push(parts[i]);
        }
    }
}