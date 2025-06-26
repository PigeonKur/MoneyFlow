function renderIncomeExpenseChart(canvasId, labels, incomeData, expenseData) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    if (ctx.chart) {
        ctx.chart.destroy();
    }

    ctx.chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Доходы',
                    data: incomeData,
                    backgroundColor: '#28a745',
                    borderRadius: 4
                },
                {
                    label: 'Расходы',
                    data: expenseData,
                    backgroundColor: '#dc3545',
                    borderRadius: 4
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                    align: 'end'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
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
}

function renderBalanceTrendChart(canvasId, labels, balanceData) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    if (ctx.chart) {
        ctx.chart.destroy();
    }

    ctx.chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Остаток на счете',
                data: balanceData,
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
                    position: 'top',
                    align: 'end'
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
}

function renderMiniPieChart(canvasId, labels, data, colors, title) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error('Canvas element not found:', canvasId);
        return;
    }

    // Проверка на пустые данные
    if (!labels || labels.length === 0 || !data || data.length === 0) {
        ctx.innerHTML = '<div class="no-data">Нет данных для отображения</div>';
        return;
    }

    if (ctx.chart) {
        ctx.chart.destroy();
    }

    // Проверка, что colors массив соответствует количеству данных
    if (colors.length < data.length) {
        colors = generateDefaultColors(data.length);
    }

    ctx.chart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors,
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '70%',
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        boxWidth: 12,
                        padding: 20
                    }
                },
                title: {
                    display: !!title,
                    text: title,
                    position: 'top',
                    padding: {
                        top: 10,
                        bottom: 20
                    }
                }
            }
        }
    });
}
function generateDefaultColors(count) {
    const defaultColors = [
        '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF',
        '#FF9F40', '#8AC24A', '#607D8B', '#E91E63', '#00BCD4'
    ];
    const colors = [];
    for (let i = 0; i < count; i++) {
        colors.push(defaultColors[i % defaultColors.length]);
    }
    return colors;
}

window.dashboardCharts = {
    renderIncomeExpenseChart,
    renderBalanceTrendChart,
    renderMiniPieChart
};