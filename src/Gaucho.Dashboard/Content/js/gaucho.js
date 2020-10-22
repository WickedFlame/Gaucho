(function (gaucho) {
    gaucho.Poller = (function(config) {
        function Poller(config) {
            poll(function () {
                    let fn = function (resolve, reject) {
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

                            // update elements here
                            response.forEach(function(pipeline) {
                                updateMetrics(pipeline.pipelineId, pipeline.metrics);
                                updateElements(pipeline.pipelineId, pipeline.elements);
                            });

                            resolve(false);
                        }).catch(function (error) {
                            console.log(error);
                            reject(error);
                        });
                    };
                    return new Promise(fn);
                },
                config.pollInterval).then(function () {
                // Polling done, now do something else!
            }).catch(function () {
                // Polling timed out, handle the error!
            });
        };

        var poll = (fn, interval) => {
            interval = interval || 1000;

            let checkCondition = function(resolve, reject) {
                fn().then(function(result) {
                    setTimeout(checkCondition, interval, resolve, reject);
                });
            };

            return new Promise(checkCondition);
        };

        var updateMetrics = (pipelineId, metrics) => {
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
        };

        var updateElements = (pipelineId, elements) => {
            for (var prop in elements) {
                if (prop.toLowerCase() === "eventlog") {
                    var element = elements[prop];
                    let elem = document.getElementById(`${pipelineId}-${element.key.toLowerCase()}`);
                    elem.innerHTML = '';

                    element.elements.forEach(e => {
                        var cls = `pipeline-log-${e.level.toLowerCase()}`;
                        var div = document.createElement('div');
                        div.innerHTML = `<div class="pipeline-item-log-item">
                                <span class="pipeline-item-log-element ${cls}">${e.timestamp}</span><span class="pipeline-item-log-element">[${e.source}]</span><span class="pipeline-item-log-element">[${e.level}]</span><span class="pipeline-item-log-element">${e.message}</span>
                            </div>`.trim();

                        // Change this to div.childNodes to support multiple top-level nodes
                        elem.appendChild(div.firstChild);
                    });

                }
            }
        };
        
        return Poller(config);
    });

})(window.Gaucho = window.Gaucho || {});

(function () {
    Gaucho.page = new Gaucho.Poller(gauchoConfig);
})();