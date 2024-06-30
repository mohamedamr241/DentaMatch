document.getElementById("getInfoForm").addEventListener("submit", function (event) {
    event.preventDefault();
    CallApi();
});
async function CallApi() {
    try {
        var id = document.getElementById("submitButtonDoctor").getAttribute("value");
        console.log(id)
        const response = await fetch('/Doctor/Statistics?userId=' + id, {
            method: 'POST'
        });
        console.log(response)
        if (!response.ok) {
            throw new Error(`Failed to verify user`);
        }
        else {
            const newData = await response.json();
            console.log(newData)
            var tar = document.getElementById("target");
            tar.innerHTML = '';
            for (var i = 0; i < newData.names.length; i++) {
                var row = document.createElement("tr");
                row.innerHTML = `
            <td>${newData.names[i]}</td>
            <td>${newData.averageNumberOfPatients[i]}</td>
        `;
                tar.appendChild(row);
            }
        }

    } catch (error) {
        console.error('Error:', error);
        // Handle error here
    }
}