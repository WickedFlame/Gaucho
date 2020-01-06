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
                            });
                            console.log(`Poll job ${response} with state ${response.state}`);

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

        function poll(fn, interval) {
            interval = interval || 1000;

            let checkCondition = function (resolve, reject) {
                fn().then(function (result) {
                    setTimeout(checkCondition, interval, resolve, reject);
                });
            };

            return new Promise(checkCondition);
        }

        function updateMetrics(pipelineId, metrics) {
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
        
        return Poller(config);
    });

})(window.Gaucho = window.Gaucho || {});

(function () {
    Gaucho.page = new Gaucho.Poller(gauchoConfig);
})();