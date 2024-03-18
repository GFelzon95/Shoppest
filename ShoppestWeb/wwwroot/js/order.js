var dataTable;

$(document).ready(function () {
    var url = window.location.search;

    switch (true)
    {
        case url.includes("pending"):
            loadDataTable("pending");
            break;
        case url.includes("inprocess"):
            loadDataTable("inprocess");
            break;
        case url.includes("completed"):
            loadDataTable("completed");
            break;
        case url.includes("approved"):
            loadDataTable("approved");
            break;
        default:
            loadDataTable();
            break;

    }
})

function loadDataTable(status) {
    dataTable = $("#orderTbl").DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: "id", width: "5%" },
            { data: "name", width: "25%" },
            { data: "phoneNumber", width: "15%" },
            { data: "applicationUser.email", width: "25%" },
            { data: "orderStatus", width: "10%" },
            {
                data: "orderTotal",
                render: function (data) {
                    return data + "$";
                },
                width: "10%"
            },
            {
                data: "id",
                render: function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/Order/Details?id=${data}" class="btn btn-primary"><i class="bi bi-pencil-square"></i></a>
                        </div>`
                },
                width:"10%"
            }
        ]
    })
}