var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $("#orderTbl").DataTable({
        "ajax": { url: '/admin/order/getall' },
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