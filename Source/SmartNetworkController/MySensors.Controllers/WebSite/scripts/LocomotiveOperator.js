Protocol = {
    DCC14: "DCC14",
    DCC28: "DCC28",
    DCC128: "DCC128"
}
//----------------------------------------------------------------------------------------------------------------------
function Locomotive(id, name, description, address, protocol) {
    this.ID = id;
    this.Name = name;
    this.Description = description;
    this.Address = address;
    this.Protocol = protocol;
}
//----------------------------------------------------------------------------------------------------------------------
function LocomotiveOperator(loco) {
    var loco = loco;
    var speed = 0;
    var forward = true;
    var sendCommand = true;
    
    var functions = [];
    for (var i = 0; i <= 28; i++)
        functions.push(false);

    // Properties:
    this.Loco = loco;// function () { return loco; }
    this.Forward = function (value) {
        if (value != null) {
            if (forward != value) {
                forward = value;
                sendLocoSpeedCommand(speed);
                //OnPropertyChanged(new PropertyChangedEventArgs("Forward"));
            }
        }
        else
            return forward;
    }
    this.Speed = function (value) {
        if (value != null) {
            //if (speed != value)
            {
                speed = (value == -1 ? 0 : value);
                sendLocoSpeedCommand(value);
                //OnPropertyChanged(new PropertyChangedEventArgs("Speed"));
            }
        }
        else
            return speed;
    }
    this.SpeedSteps = function () {
        switch (loco.Protocol) {
            case Protocol.DCC14: return 14;
            case Protocol.DCC28: return 28;
            case Protocol.DCC128: return 126;
        }
    }
    this.F = function (idx, value) { if (value != null) setFunction(idx, value); else return functions[idx]; }

    // Public functions:
    this.Stop = function () { this.Speed(-1); }
    this.Brake = function () { this.Speed(0); }
    this.Reset = function () {
        this.Stop();
        this.Forward(true);

        for (var i = 0; i <= 28; i++)
        {
            functions[i] = false;
            //OnPropertyChanged(new PropertyChangedEventArgs("F" + i.ToString()));
        }
        SendAllFunctionsCommand();
    }
    this.ToggleDirection = function () { this.Forward(!this.Forward()); }

    // Private functions:
    function setFunction(idx, value) {
        if (functions[idx] != value)
        {
            functions[idx] = value;
            if (loco) {
                if (idx == 0 && loco.Protocol == Protocol.DCC14)
                    sendLocoSpeedCommand(speed);
                else
                    sendFunctionCommand(idx);
            }
            //OnPropertyChanged(new PropertyChangedEventArgs("F" + idx.ToString()));
        }
    }
    function sendLocoSpeedCommand(speed) // speed = -1, 0,1,2,3...(int)loco.SpeedSteps
    {
        if (sendCommand)// && IsOperable)
        {
            switch (loco.Protocol) {
                case Protocol.DCC14: model.MessageManager.SetLocoSpeed14(loco.Address, speed, forward, functions[0]); break;
                case Protocol.DCC28: model.MessageManager.SetLocoSpeed28(loco.Address, speed, forward); break;
                case Protocol.DCC128: model.MessageManager.SetLocoSpeed128(loco.Address, speed, forward); break;
            }
        }
    }
    function sendFunctionCommand(idx)
    {
        if (sendCommand)// && IsOperable)
        {
            if (idx >= 0 && idx <= 4)
                model.MessageManager.SetLocoFunctionGroup1(loco.Address, functions[0], functions[1], functions[2], functions[3], functions[4]);
            else if (idx >= 5 && idx <= 8)
                model.MessageManager.SetLocoFunctionGroup2(loco.Address, functions[5], functions[6], functions[7], functions[8]);
            else if (idx >= 9 && idx <= 12)
                model.MessageManager.SetLocoFunctionGroup3(loco.Address, functions[9], functions[10], functions[11], functions[12]);
            else if (idx >= 13 && idx <= 20)
                model.MessageManager.SetLocoFunctionGroup4(loco.Address, functions[13], functions[14], functions[15], functions[16], functions[17], functions[18], functions[19], functions[20]);
            else if (idx >= 21 && idx <= 28)
                model.MessageManager.SetLocoFunctionGroup5(loco.Address, functions[21], functions[22], functions[23], functions[24], functions[25], functions[26], functions[27], functions[28]);
        }
    }
    function sendAllFunctionsCommand()
    {
        if (sendCommand)// && IsOperable)
        {
            if (loco.Protocol == Protocol.DCC14)
                model.MessageManager.SetLocoSpeed14(loco.Address, speed, forward, functions[0]);

            model.MessageManager.SetLocoFunctionGroup1(loco.Address, functions[0], functions[1], functions[2], functions[3], functions[4]);
            model.MessageManager.SetLocoFunctionGroup2(loco.Address, functions[5], functions[6], functions[7], functions[8]);
            model.MessageManager.SetLocoFunctionGroup3(loco.Address, functions[9], functions[10], functions[11], functions[12]);
            model.MessageManager.SetLocoFunctionGroup4(loco.Address, functions[13], functions[14], functions[15], functions[16], functions[17], functions[18], functions[19], functions[20]);
            model.MessageManager.SetLocoFunctionGroup5(loco.Address, functions[21], functions[22], functions[23], functions[24], functions[25], functions[26], functions[27], functions[28]);
        }
    }

    //this.Reset();
}