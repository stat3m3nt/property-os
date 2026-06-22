``` mermaid
erDiagram
    AUTH_USERS ||--|| PROFILES : "auth user profile"
    PROFILES ||--o{ USER_ROLES : "has roles"

    PROFILES ||--o{ PROPERTY_USERS : "owns/manages"
    PROPERTIES ||--o{ PROPERTY_USERS : "assigned users"

    PROPERTIES ||--o{ UNITS : "contains"
    PROPERTIES ||--o{ LEASES : "has leases"
    UNITS ||--o{ LEASES : "leased through"

    LEASES ||--o{ LEASE_TENANTS : "has tenants"
    PROFILES ||--o{ LEASE_TENANTS : "tenant on lease"

    LEASES ||--o{ RENT_CHARGES : "generates"
    UNITS ||--o{ RENT_CHARGES : "rent for unit"
    PROPERTIES ||--o{ RENT_CHARGES : "rent under property"
    PROFILES ||--o{ RENT_CHARGES : "tenant charged"

    RENT_CHARGES ||--o{ RENT_PAYMENTS : "paid by payments"
    PROFILES ||--o{ RENT_PAYMENTS : "tenant payment"

    PROPERTIES ||--o{ MAINTENANCE_TICKETS : "has tickets"
    UNITS ||--o{ MAINTENANCE_TICKETS : "ticket for unit"
    LEASES ||--o{ MAINTENANCE_TICKETS : "optional lease context"
    PROFILES ||--o{ MAINTENANCE_TICKETS : "reported by"
    PROFILES ||--o{ MAINTENANCE_TICKETS : "assigned to"

    MAINTENANCE_TICKETS ||--o{ MAINTENANCE_COMMENTS : "has comments"
    PROFILES ||--o{ MAINTENANCE_COMMENTS : "writes comments"

    PROFILES ||--o{ DOCUMENTS : "uploads"
    PROPERTIES ||--o{ DOCUMENTS : "property documents"
    UNITS ||--o{ DOCUMENTS : "unit documents"
    LEASES ||--o{ DOCUMENTS : "lease documents"
    MAINTENANCE_TICKETS ||--o{ DOCUMENTS : "ticket attachments"
    RENT_CHARGES ||--o{ DOCUMENTS : "rent receipts"

    NOTIFICATION_EVENTS ||--o{ NOTIFICATIONS : "creates"
    PROFILES ||--o{ NOTIFICATIONS : "receives"

    AUTH_USERS

    PROFILES {
        uuid id PK
        text full_name
        text email
        text phone
        timestamptz created_at
        timestamptz updated_at
    }

    USER_ROLES {
        uuid id PK
        uuid user_id FK
        app_role role
        timestamptz created_at
        timestamptz updated_at
    }

    PROPERTIES {
        uuid id PK
        text name
        text address_line1
        text city
        text state_region
        text postal_code
        text country
        boolean is_active
        timestamptz created_at
        timestamptz updated_at
    }

    PROPERTY_USERS {
        uuid id PK
        uuid property_id FK
        uuid user_id FK
        property_user_role role
        timestamptz created_at
        timestamptz updated_at
    }

    UNITS {
        uuid id PK
        uuid property_id FK
        text unit_number
        integer bedrooms
        numeric bathrooms
        numeric rent_amount
        unit_status status
        timestamptz created_at
        timestamptz updated_at
    }

    LEASES {
        uuid id PK
        uuid property_id FK
        uuid unit_id FK
        date start_date
        date end_date
        numeric rent_amount
        lease_status status
        timestamptz created_at
        timestamptz updated_at
    }

    LEASE_TENANTS {
        uuid id PK
        uuid lease_id FK
        uuid tenant_id FK
        boolean is_primary
        timestamptz created_at
        timestamptz updated_at
    }

    MAINTENANCE_TICKETS {
        uuid id PK
        uuid property_id FK
        uuid unit_id FK
        uuid lease_id FK
        uuid reported_by FK
        uuid assigned_to FK
        text title
        text description
        maintenance_priority priority
        maintenance_status status
        timestamptz resolved_at
        timestamptz created_at
        timestamptz updated_at
    }

    MAINTENANCE_COMMENTS {
        uuid id PK
        uuid ticket_id FK
        uuid author_id FK
        text body
        timestamptz created_at
        timestamptz updated_at
    }

    RENT_CHARGES {
        uuid id PK
        uuid lease_id FK
        uuid property_id FK
        uuid unit_id FK
        uuid tenant_id FK
        date billing_month
        date due_date
        numeric base_rent_amount
        numeric late_fee_amount
        numeric amount_paid
        rent_status status
        timestamptz created_at
        timestamptz updated_at
    }

    RENT_PAYMENTS {
        uuid id PK
        uuid rent_charge_id FK
        uuid tenant_id FK
        numeric amount
        timestamptz paid_at
        payment_method method
        text external_payment_reference
        timestamptz created_at
        timestamptz updated_at
    }

    DOCUMENTS {
        uuid id PK
        uuid uploaded_by FK
        uuid property_id FK
        uuid unit_id FK
        uuid lease_id FK
        uuid ticket_id FK
        uuid rent_charge_id FK
        document_type document_type
        text title
        text storage_bucket
        text storage_path
        text mime_type
        bigint file_size_bytes
        timestamptz created_at
        timestamptz updated_at
    }

    NOTIFICATION_EVENTS {
        uuid id PK
        text event_type
        text entity_type
        uuid entity_id
        jsonb payload
        timestamptz created_at
        timestamptz updated_at
    }

    NOTIFICATIONS {
        uuid id PK
        uuid recipient_id FK
        uuid event_id FK
        text title
        text body
        timestamptz read_at
        timestamptz created_at
        timestamptz updated_at
    }
 ```

