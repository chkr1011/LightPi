function turnOn() {
    $.post("api/TurnOn");
}

function turnOff() {
    $.post("api/TurnOff");
}

function pollState() {
    $.get("api/State",
        function (response) {

            var isOn = false;
            if (response != null && response.LongState != null) {
                isOn = response.LongState !== 0;
            }

            if (isOn) {
                $("#state").html("ON");
            } else {
                $("#state").html("OFF");
            }
        });

    setTimeout(function () { pollState() }, 1000);
}

pollState();