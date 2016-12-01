function turnOn() {
    $.ajax({
        url: "api/TurnOn",
        type: 'PUT'
    });
}

function turnOff() {
    $.ajax({
        url: "api/TurnOff",
        type: 'PUT'
    });
}

function pollState() {
    $.get("api/State",
        function (response) {

            var isOn = false;
            if (response != null && response.LongState != null) {
                isOn = response.LongState !== 0;
            }

            var text = "OFF";
            var style = "label-success";
            if (isOn) {
                text = "ON";
                style = "label-warning";
            }

            $("#state").html(text);
            $("#state").removeClass("label-success label-warning label-default");
            $("#state").addClass(style);
        });

    setTimeout(function () { pollState() }, 1000);
}

pollState();