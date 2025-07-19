
# 📊 Survey Basket API

واجهة برمجة تطبيقات (API) لبناء وإدارة الاستبيانات، الأسئلة، والإجابات. تم تطويرها باستخدام ASP.NET Core وتوثيقها باستخدام OpenAPI (Swagger).

---

## 🔗 رابط التوثيق (Swagger)

- Swagger UI: [https://survey-basket-mock.runasp.net/swagger/index.html](https://survey-basket-mock.runasp.net/swagger/index.html)
- Raw JSON: [https://survey-basket-mock.runasp.net/swagger/v1/swagger.json](https://survey-basket-mock.runasp.net/swagger/v1/swagger.json)

---

## 📦 نقاط النهاية الرئيسية (Endpoints)

### 🔹 Authentication
- `POST /api/Auth/Login`
- `POST /api/Auth/Register`

### 🔹 Users
- `GET /api/User`
- `GET /api/User/{id}`
- `PUT /api/User/{id}`
- `DELETE /api/User/{id}`

### 🔹 Surveys
- `GET /api/Survey`
- `GET /api/Survey/{id}`
- `POST /api/Survey`
- `PUT /api/Survey/{id}`
- `DELETE /api/Survey/{id}`

### 🔹 Questions
- `GET /api/Question`
- `GET /api/Question/{id}`
- `POST /api/Question`
- `PUT /api/Question/{id}`
- `DELETE /api/Question/{id}`

### 🔹 Options
- `GET /api/Option`
- `GET /api/Option/{id}`
- `POST /api/Option`
- `PUT /api/Option/{id}`
- `DELETE /api/Option/{id}`

### 🔹 Answers
- `GET /api/Answer`
- `GET /api/Answer/{id}`
- `POST /api/Answer`
- `PUT /api/Answer/{id}`
- `DELETE /api/Answer/{id}`

---

## 🧪 مثال على استخدام API

### 📥 تسجيل الدخول:

```http
POST /api/Auth/Login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "P@ssw0rd"
}
```

### ✅ الرد المتوقع:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```

---

## ⚙️ المتطلبات لتشغيل المشروع

- [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download)
- قاعدة بيانات SQL Server
- أدوات اختبار API مثل [Postman](https://www.postman.com/) أو `curl`

---

## 🚀 التشغيل المحلي

```bash
git clone https://github.com/your-username/survey-basket-api.git
cd survey-basket-api
dotnet restore
dotnet run
```

افتح في المتصفح:
```
http://localhost:5000/swagger
```

---

## 🔐 التوثيق والأمان

تستخدم الواجهة توثيق JWT (Bearer Token) عبر `Authorization` Header:

```http
Authorization: Bearer {your_token}
```

---

## 📄 الرخصة

هذا المشروع تحت رخصة [MIT](LICENSE).

---

## 🤝 المساهمة

مرحبًا بالمساهمات! افتح Issue أو Pull Request لأي ميزة أو تحسين ترغب بإضافته.

---

## 📧 الدعم

لأي استفسارات أو مشاكل تقنية، تواصل عبر:  
📩 **dev@surveybasket.local**

---

> تم إنشاء هذا الملف تلقائيًا من ملف OpenAPI (`swagger.json`) بواسطة [ChatGPT].
