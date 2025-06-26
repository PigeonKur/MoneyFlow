export function renderPortfolioChart(canvasId, labels, data) {
    console.log('Attempting to render portfolio chart with data:', { labels, data });

    try {
        const ctx = document.getElementById(canvasId);
        if (!ctx) {
            console.error(`Canvas element with ID ${canvasId} not found`);
            return;
        }

        if (ctx.chart) {
            ctx.chart.destroy();
        }

        if (!labels || !data || labels.length === 0 || data.length === 0) {
            console.error('Invalid data for chart:', { labels, data });
            return;
        }

        ctx.chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Стоимость портфеля',
                    data: data,
                    borderColor: '#2e86de',
                    backgroundColor: 'rgba(46, 134, 222, 0.1)',
                    borderWidth: 2,
                    tension: 0.1,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true
                    }
                },
                scales: {
                    y: {
                        beginAtZero: false,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)'
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        }
                    }
                }
            }
        });

        console.log('Portfolio chart rendered successfully');
    } catch (error) {
        console.error('Error rendering portfolio chart:', error);
    }
}

export function renderProfitabilityChart(canvasId, labels, data) {
    try {
        console.log(`Rendering chart on canvas: ${canvasId}`);
        console.log('Labels:', labels);
        console.log('Data:', data);

        const ctx = document.getElementById(canvasId);
        if (!ctx) {
            console.error(`Canvas element with ID ${canvasId} not found`);
            return;
        }

        // Destroy previous chart if it exists
        if (ctx.chart) {
            ctx.chart.destroy();
        }

        ctx.chart = new Chart(ctx.getContext('2d'), {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Доходность портфеля (%)',
                    data: data,
                    borderColor: '#4CAF50',
                    backgroundColor: 'rgba(76, 175, 80, 0.1)',
                    tension: 0.1,
                    fill: true,
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: false,
                        title: {
                            display: true,
                            text: 'Доходность (%)'
                        }
                    },
                    x: {
                        title: {
                            display: true,
                            text: 'Дата'
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error rendering profitability chart:', error);
    }
}

