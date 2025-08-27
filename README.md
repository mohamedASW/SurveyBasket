
# 📊 Survey Basket API

واجهة برمجة تطبيقات (API) لبناء وإدارة الاستبيانات، الأسئلة، والإجابات. تم تطويرها باستخدام ASP.NET Core وتوثيقها باستخدام OpenAPI (Swagger).

---

## 🔗 رابط التوثيق (Swagger)

- Swagger UI: [https://survey-basket-mock.runasp.net/swagger/index.html](https://survey-basket-mock.runasp.net/swagger/index.html)
- Raw JSON: [https://survey-basket-mock.runasp.net/swagger/v1/swagger.json](https://survey-basket-mock.runasp.net/swagger/v1/swagger.json)

---

## 📦 نقاط النهاية الرئيسية (Endpoints)

### 🔹 Authentication
- `POST /Auth/`
- `POST /Auth/register`
- `POST /Auth/refresh`
- `POST /Auth/forget-password`
- `POST /Auth/reset-password`
- `POST /Auth/confirm-email`
- `POST /Auth/resend-confirmation-email`

### 🔹 Users
- `GET /api/Users`
- `POST /api/Users`
- `GET /api/Users/{id}`
- `PUT /api/Users/{id}`
- `PUT /api/Users/{id}/toggle-status`
- `PUT /api/Users/{id}/unlock`

### 🔹 Polls
- `GET /v1/api/polls`
- `GET /v1/api/polls/{id}`
- `POST /v1/api/polls`
- `PUT /v1/api/polls/{id}`
- `PUT /v1/api/polls/{id}/toggle-status`

### 🔹 Questions
- `GET /api/Questions`
- `GET /api/Questions/{id}`
- `POST /api/Questions`
- `PUT /api/Questions/{id}`
- `PUT /api/Questions/{id}/toggle-status`

### 🔹 Account
- `GET /me`
- `PUT /me`
- `PUT /me/change-password`
- `PUT /me/change-email`
- `POST /me/confirm-change-email`
- `POST /me/confirm-new-email`

### 🔹 Roles
- `GET /api/Roles`
- `GET /api/Roles/{id}`
- `POST /api/Roles`
- `PUT /api/Roles/{id}`
- `PUT /api/Roles/{id}/toggle-status`

---

## 🧪 مثال على استخدام API

### 📥 تسجيل الدخول:

```http
POST /api/Auth
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


