🚀 Launching the Observability Stack

Start the stack with:

docker-compose up -d


Once it's running, explore the observability tools via the following endpoints:

| 🔧 Tool              | 🌐 URL                       | 📘 Description                    |
|-----------------------|------------------------------|------------------------------------|
| 📊 Prometheus        | [http://localhost:9090](http://localhost:9090) | Metrics collection & querying      |
| 📈 Prometheus Metrics| [http://localhost:9464/metrics](http://localhost:9464/metrics) | Raw metrics endpoint               |
| 🕵️‍♂️ Jaeger (Traces) | [http://localhost:16686](http://localhost:16686) | Distributed tracing UI             |
| 📚 Elasticsearch     | [http://localhost:9200](http://localhost:9200) | Log storage & search engine        |
| 🎨 Kibana            | [http://localhost:5601](http://localhost:5601) | Visualization & dashboarding       |


Here’s a conceptual flow of how the components interact:

        ┌────────────┐
        │  Your App  │
        └────┬───────┘
             │
             ▼
     ┌───────────────┐
     │ Prometheus     │◄───── Exporters
     └───────────────┘
             │
             ▼
     ┌───────────────┐
     │ Jaeger Agent   │
     └────┬──────────┘
          ▼
     ┌───────────────┐
     │ Jaeger UI      │
     └───────────────┘

             ▲
             │
     ┌───────────────┐
     │ Elasticsearch  │◄───── Log Emitters
     └────┬──────────┘
          ▼
     ┌───────────────┐
     │ Kibana         │
     └───────────────┘

🧠 Why This Stack?

This setup provides:

- 📊 Metrics with **Prometheus**
- 🕵️‍♂️ Traces with **Jaeger**
- 📚 Logs with **Elasticsearch** + 🎨 **Kibana**

Perfect for debugging, performance monitoring, and gaining deep insights into your system.