let scanJobTable = null;

$(function () {

    loadScanJobs();

});

function loadScanJobs() {

    $.get("/ScanJob/GetList", function (data) {

        if (scanJobTable != null) {

            scanJobTable.destroy();

        }

        scanJobTable = new Tabulator("#scanJobGrid", {

            layout: "fitColumns",

            height: "600px",

            placeholder: "No scan jobs found.",

            data: data,

            columns: [

                {
                    title: "Job",
                    field: "name"
                },

                {
                    title: "Source",
                    field: "source"
                },

                {
                    title: "Enabled",
                    field: "isEnabled",
                    hozAlign: "center",
                    formatter: "tickCross"
                },

                {
                    title: "Last Run",
                    field: "lastRun",
                    formatter: function (cell) {

                        return cell.getValue() ?? "-";

                    }
                },

                {
                    title: "",
                    hozAlign: "center",
                    formatter: function () {

                        return "<button class='btn btn-primary btn-sm'>Run</button>";

                    },

                    cellClick: function (e, cell) {

                        runJob(cell.getRow().getData().code);

                    }

                }

            ]

        });

    });

}

function runJob(code) {

    Swal.fire({

        title: "Run Scan Job?",
        text: "The selected scan job will be started.",
        icon: "question",

        showCancelButton: true,

        confirmButtonText: "Run",

        cancelButtonText: "Cancel"

    }).then((result) => {

        if (!result.isConfirmed)
            return;

        $.ajax({

            url: "/ScanJob/Run",

            type: "POST",

            data: {
                code: code
            },

            success: function (response) {

                if (response.success) {

                    Swal.fire({

                        icon: "success",

                        title: "Completed",

                        text: response.message

                    });

                    loadScanJobs();

                }
                else {

                    Swal.fire({

                        icon: "error",

                        title: "Error",

                        text: response.message

                    });

                }

            },

            error: function () {

                Swal.fire({

                    icon: "error",

                    title: "Error",

                    text: "Unexpected error."

                });

            }

        });

    });

}