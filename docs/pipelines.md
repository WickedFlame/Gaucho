Pipelines
---

Each pipeline runs in its own thread and has its own queue. 
Depending on the size of the queue, the pipeline starts a number of threads to process the events in the queue.