 API REST – Inventario de Productos (InventoryHub)

API REST desarrollada con **.NET 8** para la gestión de productos en un inventario, aplicando una arquitectura por capas y buenas prácticas de desarrollo backend.  
El proyecto está pensado para **uso real en un negocio de ventas**, permitiendo crear, consultar, actualizar y eliminar productos de forma segura y escalable.

Su objetivo es demostrar el uso correcto de **.NET 8**, patrones de diseño, separación de responsabilidades y buenas prácticas en el desarrollo de APIs.



## 📸 Vista del sistema

### 🔹 Detalle de producto
<p align="center">
  <img alt="image" src="https://github.com/user-attachments/assets/0aaef66b-2d0b-4b00-be41-f77caf7945fa" width="700"/>
</p>

### 🔹 Edición de Producto
<p align="center">
 <img width="700" alt="image" src="https://github.com/user-attachments/assets/c325f7be-13f1-4363-a998-5f10a82b5a3f" />
</p>
---


## 🚀 Accesos rápidos

 **API local:**  
https://localhost:{puerto}/api/products

**Documentación Swagger (OpenAPI):**  
https://localhost:{puerto}/swagger/index.html  
> Documentación interactiva que permite explorar y probar todos los endpoints directamente desde el navegador.

---

## ⚙️ Tecnologías utilizadas

- C# 12 / .NET 8  
- ASP.NET Core Web API  
- Entity Framework Core (MySQL / Pomelo)  
- AutoMapper  
- DataAnnotations para validación de modelos  
- Swagger / OpenAPI 3  
- JWT Bearer Authentication (opcional)
- MySql 8

---

##  Estado del proyecto
Proyecto funcional y listo para producción, orientado a **gestión de productos** con buenas prácticas de desarrollo backend en .NET.

---

##  Arquitectura del proyecto

El sistema sigue una **arquitectura en capas**, lo que facilita el mantenimiento, la escalabilidad y las pruebas.

**Capas implementadas:**

- **Controller**: Exposición de endpoints REST.  
- **Service**: Lógica de negocio, validaciones y orquestación.  
- **Repository**: Acceso a la base de datos.  
- **DTO**: Transferencia de datos entre capas.  
- **Mapper**: Conversión entre entidades y DTOs usando AutoMapper.  
- **Exception Handling**: Manejo global de errores para respuestas estandarizadas.  

---

## 📁 Estructura de carpetas

```text
InventoryHub
│
├── Controllers
│   └── ProductController.cs
│
├── Models
│   └── ProductEntity.cs
│
├── DTOs
│   └── ProductDTO.cs
│
├── Repositories
│   ├── IProductRepository.cs
│   └── ProductRepositoryImpl.cs
│
├── Services
│   ├── IProductService.cs
│   └── ProductServiceImpl.cs
│
├── Mapping
│   └── ProductMapper.cs
│
├── Exceptions
│   └── GlobalExceptionHandler.cs
│
└── Program.cs

```
---

🧠 Patrones y principios aplicados

MVC (Model–View–Controller): Exposición de la API a clientes externos (frontend o app móvil).

Arquitectura en capas: Separación clara de responsabilidades.

DTO (Data Transfer Object): Desacopla la capa de persistencia de la presentación.

Repository Pattern: Encapsula el acceso a datos y mantiene la lógica de negocio separada.

Service Layer / Facade Pattern: Orquesta la lógica de negocio y operaciones complejas.

AutoMapper: Facilita la conversión entre entidades y DTOs.

Inyección de dependencias: Implementada mediante el contenedor de servicios de .NET (AddScoped, AddSingleton).

Manejo global de excepciones: Para respuestas consistentes y estandarizadas con ApiResponse.

---

▶️ Endpoints y Ejecución del proyecto

Base URL: https://localhost:{puerto}/api/products

- Método	Endpoint	Descripción
- GET	/products	Obtener todos los productos
- GET	/products/{id}	Obtener producto por ID
- POST	/products	Crear un nuevo producto
- PUT	/products/{id}	Actualizar un producto existente
- DELETE	/products/{id}	Eliminar un producto

Todos los endpoints devuelven un ApiResponse<T>, estandarizando la respuesta para facilitar la integración con el frontend.
