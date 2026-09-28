/*
Crypto Notes

## Block Cipher Modes

### ECB (Electronic Codebook) — NEVER USE IN PRODUCTION
- Encrypts each block independently with the same key
- Same plaintext block → ALWAYS same ciphertext block
- Pattern leakage: encrypt an image with ECB and you can still see the shapes
- Deterministic-per-block = attacker knows when same data repeats

### CBC (Cipher Block Chaining) — Common but no integrity
- Each block XOR'd with the previous ciphertext block before encrypting
- Random IV ensures same plaintext → different ciphertext each time
- Provides confidentiality but NOT integrity
- Tampered ciphertext: decryption produces garbage or padding error
- No way to know IF data was tampered — only that padding broke

### CTR (Counter Mode) — Stream cipher behaviour
- Encrypts a counter value, XORs with plaintext
- Parallelisable, good for random access in large files
- Still no authentication — no tamper detection

### GCM (Galois/Counter Mode) — USE THIS
- CTR mode + Galois authentication field
- Produces an authentication TAG alongside ciphertext
- Any single bit flip in ciphertext or tag → decryption THROWS
- Provides BOTH confidentiality AND integrity/authenticity
- Industry standard — use AES-GCM for all new systems 


## Authentication vs Authorization

### Authentication (authn) = WHO ARE YOU?
Proving identity. The server verifies you are who you claim.
Example: POST /login with username + password.
Server checks password hash → "Yes, this is Elakkia."
Without authn, anyone can access anything.

### Authorization (authz) = WHAT CAN YOU DO?
Checking permissions after identity is confirmed.
Example: Elakkia is a Student. Students cannot access Teacher endpoints.
Server checks role claim in token → returns 403 Forbidden.
Without authz, any authenticated user can do everything.

Always in this order: authenticate FIRST, then authorize.

## Session/Cookie vs Token

### Session/Cookie
Server stores session data in memory or database.
Client gets a session ID cookie — just an identifier.
Every request: server looks up the session ID → finds user data.
Stateful: server must remember every logged-in user.
Problem: hard to scale across multiple servers.
Example: traditional web apps, banking sites.

### Token (JWT — JSON Web Token)
Server stores NOTHING.
Client gets a signed token containing claims (user id, role, expiry).
Every request: server verifies signature → reads claims from token itself.
Stateless: any server can verify without shared state.
Easy to scale: no session storage needed.
Example: REST APIs, mobile apps, microservices.
Format: Header.Payload.Signature (each part base64-encoded, separated by dots)
*/