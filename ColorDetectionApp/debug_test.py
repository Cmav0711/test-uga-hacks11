# Quick test to understand the IQR calculation
distances = [14.14, 14.14, 14.14, 664.48, 651.05, 14.14, 14.14]
distances_sorted = sorted(distances)
print("Sorted distances:", distances_sorted)

q1_idx = len(distances_sorted) // 4
q3_idx = (len(distances_sorted) * 3) // 4
print(f"Q1 index: {q1_idx}, Q3 index: {q3_idx}")

q1 = distances_sorted[q1_idx]
q3 = distances_sorted[q3_idx]
print(f"Q1: {q1}, Q3: {q3}")

iqr = q3 - q1
print(f"IQR: {iqr}")

multiplier = 1.5
upper_bound = q3 + (multiplier * iqr)
print(f"Upper bound (Q3 + 1.5*IQR): {upper_bound}")

# Check if outliers are detected
for i, d in enumerate(distances):
    status = "KEEP" if d <= upper_bound else "REMOVE"
    print(f"Distance {i}: {d:.2f} - {status}")
