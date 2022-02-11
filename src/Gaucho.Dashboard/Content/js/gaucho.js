
export class Gaucho {
	constructor(config) {
		this.bindTogglers(document);
		this.togglePipelines(document);
		setTimeout(() => {
			this.startPolling(config, (data) => this.updateStatus(data, this));
		}, 3000);
	}

	bindTogglers(elem) {
		for(const button of elem.querySelectorAll('.toggler-button')) {
            button.addEventListener('click',
                e => {
                    let elem = e.target.closest('.toggler-section');
                    elem.classList.toggle('is-open');
                });
        };
		for (const button of elem.querySelectorAll('.pipeline-item-header')) {
			button.addEventListener('click', e => {
                let elem = e.target.closest('.toggler-section');
				elem.classList.toggle('is-open');
				let id = e.target.closest('.pipeline-item').id;
                localStorage.setItem(`is-open-${id}`, elem.classList.contains('is-open'));
            });
		};
	}

	togglePipelines(elem) {
		for (const button of elem.querySelectorAll('.pipeline-item')) {
			if (localStorage.getItem(`is-open-${button.id}`) == 'false') {
				button.querySelector('.toggler-section').classList.toggle('is-open');
            }
        }
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
			    config.pollInterval)
			.then(function () {
				// Polling done, now do something else!
			}).catch(function () {
				// Polling timed out, handle the error!
			});
	}

	updateStatus(data, gaucho) {
		data.forEach(function (pipeline) {
			gaucho.updateMetrics(pipeline.serverName, pipeline.pipelineId, pipeline.metrics);
			gaucho.updateElements(pipeline.serverName, pipeline.pipelineId, pipeline.elements);
		});
	}

	updateMetrics(serverName, pipelineId, metrics) {
		metrics.forEach(function (metric) {
			let elem = document.getElementById(`${serverName}-${pipelineId}-${metric.key}`);
			if (!elem) {
				return;
			}

			if (elem.innerText === "") {
				let label = document.getElementById(`${serverName}-${pipelineId}-${metric.key}-label`);
				if (label) {
					label.innerText = metric.title;
				}
			}

			elem.innerText = metric.value;
		});
	}

	updateElements(serverName, pipelineId, elements) {
		for (var prop in elements) {
			if (prop.toLowerCase() === "eventlog") {
                let element = elements[prop];
                let id = `${serverName}-${pipelineId}-${element.key.toLowerCase()}`;
				let elem = document.getElementById(id);
				
				if (elem === undefined || elem === null) {
                    elem = this.createLogSection(id, pipelineId, serverName);
                }
				
				elem.innerHTML = '';

				element.elements.forEach(e => {
                    let cls = `pipeline-log-${e.level.toLowerCase()}`;
                    let div = document.createElement('div');
					div.innerHTML = `<div class="pipeline-item-log-item">
                                <span class="pipeline-item-log-element ${cls}">${e.timestamp
						}</span><span class="pipeline-item-log-element">[${e.source
						}]</span><span class="pipeline-item-log-element">[${e.level
						}]</span><span class="pipeline-item-log-element">${e.message}</span>
                            </div>`.trim();

					// Change this to div.childNodes to support multiple top-level nodes
					elem.appendChild(div.firstChild);
				});

			} else if (prop.toLowerCase() === "workerslog") {
                let element = elements[prop];
                let id = `${serverName}-${pipelineId}-${element.key.toLowerCase()}`;
				let elem = document.getElementById(id);

                if (elem === undefined || elem === null) {
                    elem = this.createLogSection(id, pipelineId, serverName);
				}

                elem.innerHTML = '';

                element.elements.forEach(e => {
                    let div = document.createElement('div');
					div.innerHTML = `<div class="pipeline-item-log-item">
								<span class="pipeline-item-log-element pipeline-log-debug">${e.timestamp}</span><span class="pipeline-item-log-element">${e.value}</span>
							</div>`.trim();

                    // Change this to div.childNodes to support multiple top-level nodes
                    elem.appendChild(div.firstChild);
                });
            }
		}
	}

	createLogSection(id, pipelineId, serverName) {
        var div = document.createElement('div');
        div.innerHTML = `<div><div class="toggler-section">
					    <span class="toggler-button"></span>
					    <div class="label" id="logs-label">Logs</div>
				    </div>

				    <div id="${id}" class="pipeline-item-log-list toggle-wrapper">
				    </div></div>`;
        var section = document.querySelector(`#${serverName}-${pipelineId}`).querySelector('.pipeline-item-log-container')
            .appendChild(div.firstChild);

        this.bindTogglers(section);

		return document.querySelector(`#${id}`);
    }
}

if (gauchoConfig === undefined) {
	gauchoConfig = {
		pollUrl: "/gaucho/metrics",
		pollInterval: 2000
	};
}

const gaucho = new Gaucho(gauchoConfig);
