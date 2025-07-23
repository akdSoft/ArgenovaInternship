# Giriş-Çıkış Saatlerini Analiz Eden Chatbot Uygulaması

Bu proje, bir firmadaki personellerin giriş-çıkış saatlerini analiz eden bir **yapay zeka destekli chatbot** uygulamasıdır. Kullanıcılar, geçmişe dönük sorgular yapabilir, belirli günlere ait çalışma düzenini değerlendirebilir ve dikkat çekici davranış örüntülerini keşfedebilir.

Uygulama, bağlamsal analiz gücünü artırmak amacıyla ilgili geçmiş sorguları otomatik olarak analiz prompt’una entegre eder. Bu sayede *daha isabetli ve tutarlı* sonuçlar elde edilmesi hedeflenmektedir.

## 🔧 Kullanılan Teknolojiler

Proje aşağıdaki bileşenlerden oluşur:

- **Qdrant**: Vektör benzerlik aramaları için kullanılır (embedding tabanlı bağlam araması).
- **MongoDB**: Yapısal olmayan veya değişken tipte verilerin depolanmasını sağlar.
- **Llama (Ollama üzerinden lokal olarak çalıştırılır)**: Gelişmiş yapay zeka analizleri için kullanılır. Kişisel verilerin güvenliği gözetilerek lokal çalıştırılır.
- **ASP.NET Core Web API**: Tüm servisler arası iletişimi sağlayan sunucu katmanıdır.
- **Vue.js**: Web tabanlı kullanıcı arayüzü.
- **Flutter**: Mobil cihazlar için arayüz.

## 🧩 Akış Diyagramı

Aşağıda uygulamanın genel işleyişini basitçe ifade eden bir akış diyagramı yer almaktadır:

![Uygulama Akış Diyagramı](https://github.com/akdSoft/ArgenovaInternship/blob/6704603a1342f0ff016ed51ca3ea8f7a31085214/Data-Analysis-AI/diagram.png)
