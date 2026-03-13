// Clinic<T> : D × P → A, CRUD + analytics
// σ(R, pred) = {r ∈ R | pred(r)}, O(n)
// count[key] = |{r ∈ R : π_key(r) = key}|, O(n)
// sort(R, cmp) → O(n log n)

#include <iostream>
#include <string>
#include <vector>
#include <map>
#include <unordered_map>
#include <algorithm>
#include <functional>
#include <sstream>
#include <iomanip>
#include <numeric>
#include <optional>
#include <span>
#include <ranges>
#include <utility>
#include <string_view>

struct Doctor {
    int id{};
    std::string surname;
    std::string name;
    std::string patronymic;
    std::string specialty;
    int office{};
    std::string birthdate;
    std::string phone;
};

struct Patient {
    int id{};
    std::string surname;
    std::string name;
    std::string patronymic;
    std::string birthdate;
    std::string address;
    std::string phone;
    std::string policy;
};

// D × P → A
struct Appointment {
    int id{};
    int patient_id{};
    int doctor_id{};
    std::string date;
    std::string complaint;
    std::string diagnosis;
    std::string treatment;
};

// |x| ≥ 0 → bool
constexpr bool is_positive(int x) noexcept { return x > 0; }

class Clinic {
    std::map<int, Doctor>    doctors_;
    std::map<int, Patient>   patients_;
    std::vector<Appointment> appointments_;

    int next_doctor_id_  = 1;
    int next_patient_id_ = 1;
    int next_appt_id_    = 1;

public:
    // O(1)
    [[nodiscard]] const std::map<int, Doctor>&    doctors()      const noexcept { return doctors_; }
    [[nodiscard]] const std::map<int, Patient>&   patients()     const noexcept { return patients_; }
    [[nodiscard]] const std::vector<Appointment>&  appointments() const noexcept { return appointments_; }
    [[nodiscard]] std::span<const Appointment>     appointments_span() const noexcept { return appointments_; }

    // O(log n), n = |D|
    void add_doctor(Doctor&& d) {
        d.id = next_doctor_id_++;
        doctors_.emplace(d.id, std::move(d));
    }

    // find_doctor(id) → optional<cref<D>>, O(log n)
    [[nodiscard]] std::optional<std::reference_wrapper<const Doctor>>
    find_doctor(int id) const {
        if (auto it = doctors_.find(id); it != doctors_.end())
            return std::cref(it->second);
        return std::nullopt;
    }

    // find_doctor_mut(id) → optional<ref<D>>, O(log n)
    [[nodiscard]] std::optional<std::reference_wrapper<Doctor>>
    find_doctor_mut(int id) {
        if (auto it = doctors_.find(id); it != doctors_.end())
            return std::ref(it->second);
        return std::nullopt;
    }

    // O(log n)
    [[nodiscard]] bool update_doctor(int id, Doctor&& d) {
        auto it = doctors_.find(id);
        if (it == doctors_.end()) return false;
        d.id = id;
        it->second = std::move(d);
        return true;
    }

    // O(log n + k), k = |{a ∈ A : a.doctor_id = id}|
    [[nodiscard]] bool delete_doctor(int id) {
        if (doctors_.erase(id) == 0) return false;
        std::erase_if(appointments_, [id](const Appointment& a) { return a.doctor_id == id; });
        return true;
    }

    // O(log n)
    void add_patient(Patient&& p) {
        p.id = next_patient_id_++;
        patients_.emplace(p.id, std::move(p));
    }

    // find_patient(id) → optional<cref<P>>, O(log n)
    [[nodiscard]] std::optional<std::reference_wrapper<const Patient>>
    find_patient(int id) const {
        if (auto it = patients_.find(id); it != patients_.end())
            return std::cref(it->second);
        return std::nullopt;
    }

    // find_patient_mut(id) → optional<ref<P>>, O(log n)
    [[nodiscard]] std::optional<std::reference_wrapper<Patient>>
    find_patient_mut(int id) {
        if (auto it = patients_.find(id); it != patients_.end())
            return std::ref(it->second);
        return std::nullopt;
    }

    // O(log n)
    [[nodiscard]] bool update_patient(int id, Patient&& p) {
        auto it = patients_.find(id);
        if (it == patients_.end()) return false;
        p.id = id;
        it->second = std::move(p);
        return true;
    }

    // O(log n + k)
    [[nodiscard]] bool delete_patient(int id) {
        if (patients_.erase(id) == 0) return false;
        std::erase_if(appointments_, [id](const Appointment& a) { return a.patient_id == id; });
        return true;
    }

    // O(1) amortised
    void add_appointment(Appointment&& a) {
        a.id = next_appt_id_++;
        appointments_.push_back(std::move(a));
    }

    // find_appointment(id) → optional<cref<A>>, O(k)
    [[nodiscard]] std::optional<std::reference_wrapper<const Appointment>>
    find_appointment(int id) const {
        auto it = std::ranges::find_if(appointments_, [id](const Appointment& a) { return a.id == id; });
        if (it != appointments_.end()) return std::cref(*it);
        return std::nullopt;
    }

    // find_appointment_mut(id) → optional<ref<A>>, O(k)
    [[nodiscard]] std::optional<std::reference_wrapper<Appointment>>
    find_appointment_mut(int id) {
        auto it = std::ranges::find_if(appointments_, [id](const Appointment& a) { return a.id == id; });
        if (it != appointments_.end()) return std::ref(*it);
        return std::nullopt;
    }

    // O(k)
    [[nodiscard]] bool update_appointment(int id, Appointment&& upd) {
        auto it = std::ranges::find_if(appointments_, [id](const Appointment& a) { return a.id == id; });
        if (it == appointments_.end()) return false;
        upd.id = id;
        *it = std::move(upd);
        return true;
    }

    // O(k)
    [[nodiscard]] bool delete_appointment(int id) {
        auto before = appointments_.size();
        std::erase_if(appointments_, [id](const Appointment& a) { return a.id == id; });
        return appointments_.size() < before;
    }

    // count[s] = |{d ∈ D : specialty(d) = s}|, O(n), amortised O(1) per insert
    [[nodiscard]] std::unordered_map<std::string, int> count_by_specialty() const {
        std::unordered_map<std::string, int> cnt;
        for (const auto& [id, d] : doctors_) {
            cnt[d.specialty]++;
        }
        return cnt;
    }

    // count[d] = |{a ∈ A : doctor_id(a) = d}|, O(k + n log n)
    [[nodiscard]] std::vector<std::pair<int, int>> appointments_per_doctor() const {
        std::unordered_map<int, int> cnt;
        for (const auto& a : appointments_) {
            cnt[a.doctor_id]++;
        }
        std::vector<std::pair<int, int>> result;
        result.reserve(doctors_.size());
        for (const auto& [id, d] : doctors_) {
            result.emplace_back(id, cnt[id]);
        }
        // O(n log n), desc by count
        std::ranges::sort(result, [](const auto& a, const auto& b) { return a.second > b.second; });
        return result;
    }

    // σ(D, specialty = spec), sort O(n log n), lower_bound O(log n)
    [[nodiscard]] std::vector<std::reference_wrapper<const Doctor>>
    find_doctor_by_specialty(std::string_view spec) const {
        std::vector<std::reference_wrapper<const Doctor>> result;
        for (const auto& [id, d] : doctors_) {
            if (d.specialty == spec) result.emplace_back(std::cref(d));
        }
        return result;
    }

    // σ(A, patient_id = pid), O(k log k)
    [[nodiscard]] std::vector<Appointment>
    patient_history(int patient_id) const {
        std::vector<Appointment> hist;
        for (const auto& a : std::span<const Appointment>(appointments_)) {
            if (a.patient_id == patient_id) {
                hist.push_back(a);
            }
        }
        // O(m log m)
        std::ranges::sort(hist, [](const Appointment& a, const Appointment& b) {
            return a.date < b.date;
        });
        return hist;
    }

    // π(doctor, A) ⋈ D, ORDER BY count DESC, O(k + n log n)
    [[nodiscard]] std::vector<std::pair<Doctor, int>> doctors_by_appointment_count() const {
        auto counts = appointments_per_doctor();
        std::vector<std::pair<Doctor, int>> result;
        result.reserve(counts.size());
        for (const auto& [did, cnt] : counts) {
            if (auto it = doctors_.find(did); it != doctors_.end()) {
                result.emplace_back(it->second, cnt);
            }
        }
        return result;
    }

    // σ(A, date = d), O(k)
    [[nodiscard]] std::vector<Appointment>
    filter_by_date(std::string_view date) const {
        std::vector<Appointment> res;
        for (const auto& a : std::span<const Appointment>(appointments_)) {
            if (a.date == date) res.push_back(a);
        }
        return res;
    }

    // σ(A, doctor_id = did), O(k)
    [[nodiscard]] std::vector<Appointment>
    filter_by_doctor(int doctor_id) const {
        std::vector<Appointment> res;
        for (const auto& a : std::span<const Appointment>(appointments_)) {
            if (a.doctor_id == doctor_id) res.push_back(a);
        }
        return res;
    }

    // stable_sort (date, doctor_id), O(k log k)
    [[nodiscard]] std::vector<Appointment> sorted_appointments_by_date_doctor() const {
        auto copy = appointments_;
        std::ranges::sort(copy, [](const Appointment& a, const Appointment& b) {
            if (a.date != b.date) return a.date < b.date;
            return a.doctor_id < b.doctor_id;
        });
        return copy;
    }

    // σ(P, surname.prefix = q), O(n)
    [[nodiscard]] std::vector<std::reference_wrapper<const Patient>>
    find_patients_by_surname(std::string_view prefix) const {
        std::vector<std::reference_wrapper<const Patient>> res;
        for (const auto& [id, p] : patients_) {
            if (p.surname.starts_with(prefix)) {
                res.emplace_back(std::cref(p));
            }
        }
        return res;
    }
};

static void print_doctor(const Doctor& d) {
    std::cout << "  [" << d.id << "] "
              << d.surname << " " << d.name << " " << d.patronymic
              << " | " << d.specialty
              << " | office " << d.office
              << " | " << d.birthdate
              << " | " << d.phone << "\n";
}

static void print_patient(const Patient& p) {
    std::cout << "  [" << p.id << "] "
              << p.surname << " " << p.name << " " << p.patronymic
              << " | " << p.birthdate
              << " | " << p.address
              << " | " << p.phone
              << " | policy: " << p.policy << "\n";
}

static void print_appointment(const Appointment& a, const Clinic& c) {
    std::string doc_name = "???";
    std::string pat_name = "???";
    if (auto opt = c.find_doctor(a.doctor_id); opt)
        doc_name = opt->get().surname + " " + opt->get().name.substr(0, 1) + ".";
    if (auto opt = c.find_patient(a.patient_id); opt)
        pat_name = opt->get().surname + " " + opt->get().name.substr(0, 1) + ".";

    std::cout << "  [" << a.id << "] "
              << a.date << " | doc: " << doc_name << " (ID=" << a.doctor_id << ")"
              << " | patient: " << pat_name << " (ID=" << a.patient_id << ")"
              << "\n        complaint: " << a.complaint
              << "\n        diagnosis: " << a.diagnosis
              << "\n        treatment: " << a.treatment << "\n";
}

[[nodiscard]] static std::string read_line(std::string_view prompt) {
    std::cout << prompt;
    std::string s;
    std::getline(std::cin, s);
    return s;
}

[[nodiscard]] static std::optional<int> read_int(std::string_view prompt) {
    std::cout << prompt;
    std::string s;
    std::getline(std::cin, s);
    try { return std::stoi(s); }
    catch (...) { return std::nullopt; }
}

// read_int_or(prompt, default) → int
[[nodiscard]] static int read_int_or(std::string_view prompt, int def) {
    auto opt = read_int(prompt);
    return opt.value_or(def);
}

static void populate(Clinic& c) {
    c.add_doctor({0, "Ivanov",    "Alexey",   "Petrovich",    "Therapist",       101, "1975-03-12", "+7-495-111-0001"});
    c.add_doctor({0, "Petrova",   "Elena",    "Sergeevna",    "Cardiologist",    205, "1980-07-25", "+7-495-111-0002"});
    c.add_doctor({0, "Sidorov",   "Dmitry",   "Ivanovich",    "Surgeon",         310, "1972-11-08", "+7-495-111-0003"});
    c.add_doctor({0, "Kozlova",   "Anna",     "Vladimirovna", "Neurologist",     112, "1985-01-30", "+7-495-111-0004"});
    c.add_doctor({0, "Smirnov",   "Igor",     "Nikolaevich",  "Therapist",       102, "1978-09-14", "+7-495-111-0005"});
    c.add_doctor({0, "Vasilyeva", "Olga",     "Andreevna",    "Ophthalmologist", 408, "1983-05-21", "+7-495-111-0006"});
    c.add_doctor({0, "Novikov",   "Sergey",   "Alexandrovich","Cardiologist",     206, "1976-12-03", "+7-495-111-0007"});
    c.add_doctor({0, "Morozova",  "Tatyana",  "Dmitrievna",   "Dermatologist",   115, "1990-04-17", "+7-495-111-0008"});

    c.add_patient({0, "Kuznetsov",  "Mikhail",   "Olegovich",    "1990-02-15", "Lenina st. 10, apt 5",        "+7-903-200-0001", "7712345678901234"});
    c.add_patient({0, "Popova",     "Maria",     "Ivanovna",     "1985-06-22", "Mira ave. 45, apt 12",        "+7-903-200-0002", "7712345678901235"});
    c.add_patient({0, "Volkov",     "Andrey",    "Sergeevich",   "1978-11-03", "Gagarina st. 7, apt 88",      "+7-903-200-0003", "7712345678901236"});
    c.add_patient({0, "Sokolova",   "Ekaterina", "Pavlovna",     "1995-08-19", "Pushkina st. 23, apt 1",      "+7-903-200-0004", "7712345678901237"});
    c.add_patient({0, "Lebedev",    "Pavel",     "Andreevich",   "2000-01-07", "Chekhova st. 3, apt 56",      "+7-903-200-0005", "7712345678901238"});
    c.add_patient({0, "Kozlov",     "Nikolay",   "Fedorovich",   "1965-04-30", "Pobedy ave. 100, apt 33",     "+7-903-200-0006", "7712345678901239"});
    c.add_patient({0, "Novikova",   "Svetlana",  "Gennadievna",  "1988-12-11", "Sadovaya st. 15, apt 7",      "+7-903-200-0007", "7712345678901240"});
    c.add_patient({0, "Morozov",    "Viktor",    "Yurievich",    "1970-09-28", "Kirova st. 42, apt 19",       "+7-903-200-0008", "7712345678901241"});
    c.add_patient({0, "Egorova",    "Irina",     "Vasilievna",   "1992-03-05", "Lermontova st. 8, apt 64",    "+7-903-200-0009", "7712345678901242"});
    c.add_patient({0, "Fedorov",    "Alexey",    "Timofeevich",  "1982-07-16", "Kosmonavtov ave. 55, apt 2",  "+7-903-200-0010", "7712345678901243"});
    c.add_patient({0, "Alexandrov", "Denis",     "Romanovich",   "1998-10-25", "Sovetskaya st. 31, apt 40",   "+7-903-200-0011", "7712345678901244"});
    c.add_patient({0, "Belova",     "Natalya",   "Maksimovna",   "1975-05-09", "Shkolnaya st. 17, apt 11",    "+7-903-200-0012", "7712345678901245"});

    c.add_appointment({0, 1, 1, "2025-09-01", "Headache, weakness",                   "ARVI",                               "Paracetamol, bed rest"});
    c.add_appointment({0, 2, 2, "2025-09-02", "Chest pain",                           "Angina pectoris",                    "Nitroglycerin, ECG monitoring"});
    c.add_appointment({0, 3, 3, "2025-09-03", "Right side pain",                      "Appendicitis",                       "Emergency appendectomy"});
    c.add_appointment({0, 4, 4, "2025-09-04", "Numbness in hands, dizziness",         "Cervical osteochondrosis",           "Massage, exercise, ibuprofen"});
    c.add_appointment({0, 5, 1, "2025-09-05", "Cough, temperature 38.5",              "Bronchitis",                         "Amoxicillin, mucolytics"});
    c.add_appointment({0, 6, 5, "2025-09-06", "General weakness, drowsiness",         "Anemia",                             "Iron supplements, diet"});
    c.add_appointment({0, 7, 6, "2025-09-07", "Vision loss",                          "Moderate myopia",                    "Spectacle correction -3.5D"});
    c.add_appointment({0, 8, 7, "2025-09-08", "Dyspnea, leg edema",                   "Chronic heart failure",              "Diuretics, ACE inhibitors"});
    c.add_appointment({0, 9, 8, "2025-09-09", "Skin rash on hands",                   "Contact dermatitis",                 "Antihistamines, hydrocortisone"});
    c.add_appointment({0, 10, 1, "2025-09-10","Sore throat, runny nose",              "ARVI",                               "Gargling, symptomatic treatment"});
    c.add_appointment({0, 1, 2, "2025-09-12", "Rapid heartbeat",                      "Tachycardia",                        "Bisoprolol, ECG control"});
    c.add_appointment({0, 11, 3, "2025-09-13","Knee injury",                          "Meniscus tear",                      "Arthroscopy, rehabilitation"});
    c.add_appointment({0, 12, 4, "2025-09-14","Insomnia, anxiety",                    "Generalized anxiety disorder",       "Sedatives, psychotherapy"});
    c.add_appointment({0, 2, 5, "2025-09-15", "Temperature, body aches",              "Influenza",                          "Oseltamivir, fluids"});
    c.add_appointment({0, 3, 1, "2025-09-16", "Back pain",                            "Lumbalgia",                          "NSAIDs, physiotherapy"});
    c.add_appointment({0, 5, 6, "2025-09-17", "Eye redness, itching",                 "Allergic conjunctivitis",            "Antihistamine drops"});
    c.add_appointment({0, 6, 7, "2025-09-18", "Pressing chest pain",                  "IHD",                                "Aspirin, statins, coronarography"});
    c.add_appointment({0, 8, 8, "2025-09-19", "Itchy spots on back",                  "Psoriasis",                          "Methotrexate, UV therapy"});
    c.add_appointment({0, 4, 1, "2025-09-20", "Headache, tinnitus",                   "Arterial hypertension",              "Lisinopril, BP monitoring"});
    c.add_appointment({0, 9, 2, "2025-09-21", "Heart rhythm irregularities",          "Atrial fibrillation",                "Amiodarone, anticoagulants"});
}

static void menu_crud_doctor(Clinic& c) {
    std::cout << "\n=== CRUD: Doctors ===\n"
              << "  1. Add\n  2. Update\n  3. Delete\n  0. Back\n";
    const int ch = read_int_or("> ", 0);
    if (ch == 1) {
        Doctor d{};
        d.surname    = read_line("Surname: ");
        d.name       = read_line("Name: ");
        d.patronymic = read_line("Patronymic: ");
        d.specialty  = read_line("Specialty: ");
        d.office     = read_int_or("Office: ", 0);
        d.birthdate  = read_line("Birthdate (YYYY-MM-DD): ");
        d.phone      = read_line("Phone: ");
        c.add_doctor(std::move(d));
        std::cout << "Doctor added.\n";
    } else if (ch == 2) {
        const int id = read_int_or("Doctor ID: ", -1);
        auto opt = c.find_doctor(id);
        if (!opt) { std::cout << "Not found.\n"; return; }
        print_doctor(opt->get());
        Doctor d = opt->get();
        std::string s;
        s = read_line("Surname [" + d.surname + "]: ");      if (!s.empty()) d.surname = std::move(s);
        s = read_line("Name [" + d.name + "]: ");             if (!s.empty()) d.name = std::move(s);
        s = read_line("Patronymic [" + d.patronymic + "]: "); if (!s.empty()) d.patronymic = std::move(s);
        s = read_line("Specialty [" + d.specialty + "]: ");    if (!s.empty()) d.specialty = std::move(s);
        const int off = read_int_or("Office [" + std::to_string(d.office) + "]: ", 0);
        if (is_positive(off)) d.office = off;
        s = read_line("Birthdate [" + d.birthdate + "]: ");    if (!s.empty()) d.birthdate = std::move(s);
        s = read_line("Phone [" + d.phone + "]: ");            if (!s.empty()) d.phone = std::move(s);
        c.update_doctor(id, std::move(d));
        std::cout << "Updated.\n";
    } else if (ch == 3) {
        const int id = read_int_or("Doctor ID to delete: ", -1);
        std::cout << (c.delete_doctor(id) ? "Deleted.\n" : "Not found.\n");
    }
}

static void menu_crud_patient(Clinic& c) {
    std::cout << "\n=== CRUD: Patients ===\n"
              << "  1. Add\n  2. Update\n  3. Delete\n  0. Back\n";
    const int ch = read_int_or("> ", 0);
    if (ch == 1) {
        Patient p{};
        p.surname    = read_line("Surname: ");
        p.name       = read_line("Name: ");
        p.patronymic = read_line("Patronymic: ");
        p.birthdate  = read_line("Birthdate (YYYY-MM-DD): ");
        p.address    = read_line("Address: ");
        p.phone      = read_line("Phone: ");
        p.policy     = read_line("Policy: ");
        c.add_patient(std::move(p));
        std::cout << "Patient added.\n";
    } else if (ch == 2) {
        const int id = read_int_or("Patient ID: ", -1);
        auto opt = c.find_patient(id);
        if (!opt) { std::cout << "Not found.\n"; return; }
        print_patient(opt->get());
        Patient p = opt->get();
        std::string s;
        s = read_line("Surname [" + p.surname + "]: ");       if (!s.empty()) p.surname = std::move(s);
        s = read_line("Name [" + p.name + "]: ");              if (!s.empty()) p.name = std::move(s);
        s = read_line("Patronymic [" + p.patronymic + "]: ");  if (!s.empty()) p.patronymic = std::move(s);
        s = read_line("Birthdate [" + p.birthdate + "]: ");    if (!s.empty()) p.birthdate = std::move(s);
        s = read_line("Address [" + p.address + "]: ");        if (!s.empty()) p.address = std::move(s);
        s = read_line("Phone [" + p.phone + "]: ");            if (!s.empty()) p.phone = std::move(s);
        s = read_line("Policy [" + p.policy + "]: ");          if (!s.empty()) p.policy = std::move(s);
        c.update_patient(id, std::move(p));
        std::cout << "Updated.\n";
    } else if (ch == 3) {
        const int id = read_int_or("Patient ID to delete: ", -1);
        std::cout << (c.delete_patient(id) ? "Deleted.\n" : "Not found.\n");
    }
}

static void menu_crud_appointment(Clinic& c) {
    std::cout << "\n=== CRUD: Appointments ===\n"
              << "  1. Add\n  2. Update\n  3. Delete\n  0. Back\n";
    const int ch = read_int_or("> ", 0);
    if (ch == 1) {
        Appointment a{};
        a.patient_id = read_int_or("Patient ID: ", -1);
        if (!c.find_patient(a.patient_id)) { std::cout << "Patient not found.\n"; return; }
        a.doctor_id  = read_int_or("Doctor ID: ", -1);
        if (!c.find_doctor(a.doctor_id))   { std::cout << "Doctor not found.\n"; return; }
        a.date       = read_line("Date (YYYY-MM-DD): ");
        a.complaint  = read_line("Complaint: ");
        a.diagnosis  = read_line("Diagnosis: ");
        a.treatment  = read_line("Treatment: ");
        c.add_appointment(std::move(a));
        std::cout << "Appointment added.\n";
    } else if (ch == 2) {
        const int id = read_int_or("Appointment ID: ", -1);
        auto opt = c.find_appointment(id);
        if (!opt) { std::cout << "Not found.\n"; return; }
        print_appointment(opt->get(), c);
        Appointment a = opt->get();
        int v = read_int_or("Patient ID [" + std::to_string(a.patient_id) + "]: ", 0);
        if (is_positive(v)) { if (!c.find_patient(v)) { std::cout << "Not found.\n"; return; } a.patient_id = v; }
        v = read_int_or("Doctor ID [" + std::to_string(a.doctor_id) + "]: ", 0);
        if (is_positive(v)) { if (!c.find_doctor(v))  { std::cout << "Not found.\n"; return; } a.doctor_id = v; }
        std::string s;
        s = read_line("Date [" + a.date + "]: ");          if (!s.empty()) a.date = std::move(s);
        s = read_line("Complaint [" + a.complaint + "]: "); if (!s.empty()) a.complaint = std::move(s);
        s = read_line("Diagnosis [" + a.diagnosis + "]: "); if (!s.empty()) a.diagnosis = std::move(s);
        s = read_line("Treatment [" + a.treatment + "]: "); if (!s.empty()) a.treatment = std::move(s);
        c.update_appointment(id, std::move(a));
        std::cout << "Updated.\n";
    } else if (ch == 3) {
        const int id = read_int_or("Appointment ID to delete: ", -1);
        std::cout << (c.delete_appointment(id) ? "Deleted.\n" : "Not found.\n");
    }
}

int main() {
#ifdef _WIN32
    std::system("chcp 65001 > nul 2>&1");
#endif

    Clinic clinic;
    populate(clinic);

    std::cout << "======================================\n"
              << "   CLINIC — Information System\n"
              << "   C++20, in-memory, STL\n"
              << "======================================\n";

    bool running = true;
    while (running) {
        std::cout << "\n-------------- MENU --------------\n"
                  << "  1. Doctors: list / search by specialty\n"
                  << "  2. Patients: list / search by surname\n"
                  << "  3. Appointments: view / filters\n"
                  << "  4. Reports\n"
                  << "  5. Patient history\n"
                  << "  6. CRUD\n"
                  << "  0. Exit\n";

        const int choice = read_int_or("> ", -1);

        switch (choice) {
        case 1: {
            std::cout << "\n  1. Show all doctors\n  2. Search by specialty\n";
            const int sub = read_int_or("> ", 0);
            if (sub == 1) {
                std::cout << "\n--- All doctors (" << clinic.doctors().size() << ") ---\n";
                for (const auto& [id, d] : clinic.doctors()) print_doctor(d);
            } else if (sub == 2) {
                const std::string spec = read_line("Specialty: ");
                auto found = clinic.find_doctor_by_specialty(spec);
                std::cout << "\n--- Found: " << found.size() << " ---\n";
                for (const auto& dref : found) print_doctor(dref.get());
            }
            break;
        }
        case 2: {
            std::cout << "\n  1. Show all patients\n  2. Search by surname\n";
            const int sub = read_int_or("> ", 0);
            if (sub == 1) {
                std::cout << "\n--- All patients (" << clinic.patients().size() << ") ---\n";
                for (const auto& [id, p] : clinic.patients()) print_patient(p);
            } else if (sub == 2) {
                const std::string q = read_line("Surname (prefix): ");
                auto found = clinic.find_patients_by_surname(q);
                std::cout << "\n--- Found: " << found.size() << " ---\n";
                for (const auto& pref : found) print_patient(pref.get());
            }
            break;
        }
        case 3: {
            std::cout << "\n  1. All appointments (sorted: date -> doctor)\n"
                      << "  2. Filter by date\n"
                      << "  3. Filter by doctor (ID)\n"
                      << "  4. Filter by patient (ID)\n";
            const int sub = read_int_or("> ", 0);
            if (sub == 1) {
                // O(k log k)
                auto sorted = clinic.sorted_appointments_by_date_doctor();
                std::cout << "\n--- Appointments (" << sorted.size() << "), sorted by date/doctor ---\n";
                for (const auto& a : sorted) print_appointment(a, clinic);
            } else if (sub == 2) {
                const std::string d = read_line("Date (YYYY-MM-DD): ");
                auto res = clinic.filter_by_date(d);
                std::cout << "\n--- Found: " << res.size() << " ---\n";
                for (const auto& a : res) print_appointment(a, clinic);
            } else if (sub == 3) {
                const int did = read_int_or("Doctor ID: ", -1);
                auto res = clinic.filter_by_doctor(did);
                std::cout << "\n--- Found: " << res.size() << " ---\n";
                for (const auto& a : res) print_appointment(a, clinic);
            } else if (sub == 4) {
                const int pid = read_int_or("Patient ID: ", -1);
                auto res = clinic.patient_history(pid);
                std::cout << "\n--- Found: " << res.size() << " ---\n";
                for (const auto& a : res) print_appointment(a, clinic);
            }
            break;
        }
        case 4: {
            std::cout << "\n  1. Load per doctor (appointment count, desc)\n"
                      << "  2. Statistics by specialty\n";
            const int sub = read_int_or("> ", 0);
            if (sub == 1) {
                // O(k + n log n)
                auto stats = clinic.doctors_by_appointment_count();
                std::cout << "\n--- Load per doctor ---\n";
                for (const auto& [doc, cnt] : stats) {
                    std::cout << "  " << doc.surname << " " << doc.name << " " << doc.patronymic
                              << " (" << doc.specialty << ") -- "
                              << cnt << " appointment(s)\n";
                }
            } else if (sub == 2) {
                // O(n)
                auto cnt = clinic.count_by_specialty();
                std::vector<std::pair<std::string, int>> sorted(cnt.begin(), cnt.end());
                std::ranges::sort(sorted, [](const auto& a, const auto& b) { return a.second > b.second; });
                std::cout << "\n--- Doctors by specialty ---\n";
                for (const auto& [spec, n] : sorted) {
                    std::cout << "  " << spec << ": " << n << "\n";
                }
            }
            break;
        }
        case 5: {
            const int pid = read_int_or("Patient ID: ", -1);
            auto opt = clinic.find_patient(pid);
            if (!opt) { std::cout << "Patient not found.\n"; break; }
            std::cout << "\n--- Patient history ---\n";
            print_patient(opt->get());
            // O(k log k)
            auto hist = clinic.patient_history(pid);
            if (hist.empty()) {
                std::cout << "  No appointments.\n";
            } else {
                std::cout << "  Appointments: " << hist.size() << "\n";
                for (const auto& a : hist) print_appointment(a, clinic);
            }
            break;
        }
        case 6: {
            std::cout << "\n  1. Doctors\n  2. Patients\n  3. Appointments\n";
            const int sub = read_int_or("> ", 0);
            if (sub == 1) menu_crud_doctor(clinic);
            else if (sub == 2) menu_crud_patient(clinic);
            else if (sub == 3) menu_crud_appointment(clinic);
            break;
        }
        case 0:
            running = false;
            std::cout << "Exit.\n";
            break;
        default:
            std::cout << "Invalid choice.\n";
        }
    }

    return 0;
}
