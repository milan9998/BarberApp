using Microsoft.AspNetCore.Mvc;

namespace Hair.Api.Controllers;

[ApiController]
[Route("chart")]
public class ChartController : ApiBaseController
{
    [HttpGet("")]
        public IActionResult Index()
        {
             return Content(@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Zakazivanja po mesecima</title>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
            </head>
            <body>
                <h2>Broj zakazivanja po mesecima</h2>
                <canvas id='appointmentsChart' width='400' height='400'></canvas>
<canvas id='barberChart' width='400' height='400'></canvas>

                <script>
                    async function fetchAppointments() {
                        try {
                            const response = await fetch('/schedule/appointments-per-month');
                            const data = await response.json();
                            console.log('Podaci:', data);

                            const monthNames = ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun', 'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'];
                            const labels = data.map(item => monthNames[item.month - 1] + ' ' + item.year);
                            const counts = data.map(item => item.count); 

                            const ctx = document.getElementById('appointmentsChart').getContext('2d');
                            new Chart(ctx, {
                                type: 'bar',
                                data: {
                                    labels: labels,
                                    datasets: [{
                                        label: 'Zakazivanja',
                                        data: counts,
                                        backgroundColor: 'rgba(75, 192, 192, 0.5)',
                                        borderColor: 'rgba(75, 192, 192, 1)',
                                        borderWidth: 1
                                    }]
                                },
                                options: {
                                    responsive: true,
                                    scales: { y: { beginAtZero: true } }
                                }
                            });

                        } catch (error) {
                            console.error('Greška pri dohvaćanju podataka:', error);
                        }
                    }

                    fetchAppointments();

                    async function fetchAppointments2() {
                        try {
                            const response = await fetch('/schedule/appointments-per-barber');
                            const data = await response.json();
                            console.log('Podaci:', data);

                            // Hardkodovana imena frizera
                            const barberNames = {
                                '31c6e625-a0e1-41f8-947a-4075794ee5c8': 'Marko',
                                '963c608d-a98e-43d7-929c-be4eb37f0139': 'Jovan',
                                'ca6f7e41-127a-4a4d-bd10-900fa0817979': 'Nikola'
                            };

                            // Priprema podataka za Chart.js
                            const labels = data.map(item => barberNames[item.barberId] || 'Nepoznat frizer'); 
                            const appointments = data.map(item => item.count);

                            const ctx = document.getElementById('barberChart').getContext('2d');
                            new Chart(ctx, {
                                type: 'bar',
                                data: {
                                    labels: labels, // Prikazuje imena umesto ID-a
                                    datasets: [{
                                        label: 'Zakazivanja',
                                        data: appointments, 
                                        backgroundColor: 'rgba(54, 162, 235, 0.5)',
                                        borderColor: 'rgba(54, 162, 235, 1)',
                                        borderWidth: 1
                                    }]
                                },
                                options: {
                                    responsive: true,
                                    scales: { y: { beginAtZero: true } }
                                }
                            });

                        } catch (error) {
                            console.error('Greška pri dohvaćanju podataka:', error);
                        }
                    }

                    fetchAppointments2();
                </script>
            </body>
            </html>
        ", "text/html");
        }
}