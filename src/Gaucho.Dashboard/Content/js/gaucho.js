
export class Gaucho {
	constructor(config) {
		this.bindTogglers(document);

		setTimeout(() => {
			this.startPolling(config, (data) => this.updateStatus(data, this));
		}, 3000);



		this.valores = [150, 50, 62, 34, 45, 190, 230, 220, 170, 150, 73, 54, 240, 214, 210, 240, 214, 210, 92];
		this.draw();
	}

	bindTogglers(elem) {
		Array.from(elem.querySelectorAll('.toggler-button')).forEach(elem => {
			elem.addEventListener('click',
				e => {
					var elem = e.target.closest('.toggler-section');
					elem.classList.toggle('is-open');
				});
		});
	}

	poll(fn, interval) {
		interval = interval || 1000;

		var checkCondition = function(resolve, reject) {
			fn().then(function(result) {
				setTimeout(checkCondition, interval, resolve, reject);
			});
		};

		return new Promise(checkCondition);
	}

	startPolling(config, func) {
		this.poll(function () {
					var fn = function (resolve, reject) {
						var url = config.pollUrl;
						fetch(url,
							{
								method: "GET",
								headers: { 'content-type': 'application/json;  charset=utf-8' }
							}
						).then(function (response) {
							if (!response.ok) {
								throw Error(response.statusText);
							}
							return response.json();
						}).then(function (response) {

							func(response);
							resolve(false);
						}).catch(function (error) {
							console.log(error);
							reject(error);
						});
					};
					return new Promise(fn);
				},
				1000)
			.then(function () {
				// Polling done, now do something else!
			}).catch(function () {
				// Polling timed out, handle the error!
			});
	}

	updateStatus(data, gaucho) {
		data.forEach(function (pipeline) {
			gaucho.updateMetrics(pipeline.pipelineId, pipeline.metrics);
			gaucho.updateElements(pipeline.pipelineId, pipeline.elements);
		});
	}

	updateMetrics(pipelineId, metrics) {
		metrics.forEach(function(metric) {
			let elem = document.getElementById(`${pipelineId}-${metric.key}`);
			if (!elem) {
				return;
			}

			if (elem.innerText === "") {
				let label = document.getElementById(`${pipelineId}-${metric.key}-label`);
				if (label) {
					label.innerText = metric.title;
				}
			}

			elem.innerText = metric.value;
		});
	}

	updateElements(pipelineId, elements) {
		for (var prop in elements) {
			if (prop.toLowerCase() === "eventlog") {
				var element = elements[prop];
				var pipelineLog = `${pipelineId}-${element.key.toLowerCase()}`;
				let elem = document.getElementById(pipelineLog);


				if (elem === undefined || elem === null) {
					var div = document.createElement('div');
					div.innerHTML = `<div><div class="toggler-section">
					    <span class="toggler-button"></span>
					    <div class="label" id="logs-label">Logs</div>
				    </div>

				    <div id="${pipelineLog}" class="pipeline-item-log-list toggle-wrapper">
				    </div></div>`;
					var section = document.querySelector(`#${pipelineId}`).querySelector('.pipeline-item-log-container')
						.appendChild(div.firstChild);

					this.bindTogglers(section);

					elem = document.querySelector(`#${pipelineLog}`);
				}


				elem.innerHTML = '';

				element.elements.forEach(e => {
					var cls = `pipeline-log-${e.level.toLowerCase()}`;
					var div = document.createElement('div');
					div.innerHTML = `<div class="pipeline-item-log-item">
                                <span class="pipeline-item-log-element ${cls}">${e.timestamp
						}</span><span class="pipeline-item-log-element">[${e.source
						}]</span><span class="pipeline-item-log-element">[${e.level
						}]</span><span class="pipeline-item-log-element">${e.message}</span>
                            </div>`.trim();

					// Change this to div.childNodes to support multiple top-level nodes
					elem.appendChild(div.firstChild);
				});

			}
		}
	}

	// https://codepen.io/mr_brunocastro/pen/GJRqJa
drawGrid(width, height, colun, line) {
	var ctx = document.getElementById('canvas').getContext('2d');
	ctx.fillRect(0, 0, width, height);
	ctx.save();
	for (var c = 1; c < (width / colun); c++) {
		ctx.beginPath();
		ctx.moveTo(c * colun, 0);
		ctx.lineTo(c * colun, height);
		ctx.stroke();
	}
	for (var l = 1; l < (height / line); l++) {
		ctx.beginPath();
		ctx.moveTo(0, l * line);
		ctx.lineTo(width, l * line);
		ctx.stroke();
	}
}

drawingLines(width, points, c) {
	var ctx = document.getElementById('canvas').getContext('2d');
	ctx.beginPath();
	ctx.globalAlpha = c * 0.04;
	ctx.moveTo(((c - 1) * width + 10), points[c - 1]);
	ctx.lineTo(c * width + 10, points[c]);
	ctx.lineTo(c * width + 10, 300);
	ctx.lineTo(((c - 1) * width + 10), 300);
	ctx.lineTo(((c - 1) * width + 10), points[c - 1]);
	ctx.fill();
	ctx.beginPath();
	ctx.globalAlpha = 1;
	ctx.moveTo(((c - 1) * width + 10), points[c - 1]);
	ctx.lineTo(c * width + 10, points[c]);
	ctx.stroke();
	ctx.beginPath();
	ctx.save();
	ctx.fillStyle = ctx.strokeStyle;
	ctx.arc(c * width + 10, points[c], 3, 0, Math.PI * 2);
	ctx.fill();
	ctx.restore();
}

draw() {
	var ctx = document.getElementById('canvas').getContext('2d');

	//////////////// setupBackground
	ctx.fillStyle = "#1d1f20";
	ctx.strokeStyle = "#333";
	ctx.save();
	this.drawGrid(500, 300, 10, 10);


	for (var c = 0; c < this.valores.length; c++) {
		//if (isNaN(this.pontos[c])) {
		//	this.pontos[c] = 300;
		//}
		//ctx.lineWidth = 1.4;
		var larg = 480 / (this.valores.length - 1);
		//this.diferenca[c] = (300 - this.valores[c]) - this.pontos[c];
		//this.pontos[c] += this.diferenca[c] / (c + 1);

		////////////////// setupGraphic
		//ctx.strokeStyle = "#0ff";
		//ctx.fillStyle = "#0ff";
		//this.drawingLines(larg, this.pontos, c);
		this.drawingLines(larg, this.valores, c);
	}
}
}

if (gauchoConfig === undefined) {
	gauchoConfig = {
		pollUrl: "/gaucho/metrics",
		pollInterval: 2000
	};
}

const gaucho = new Gaucho(gauchoConfig);
