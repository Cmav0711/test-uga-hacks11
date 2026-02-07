import math

# Test 7 points
points = [(0,0), (10,10), (500,500), (20,20), (30,30), (600,100), (40,40), (50,50)]

# Calculate distances
distances = []
for i in range(1, len(points)):
    dx = points[i][0] - points[i-1][0]
    dy = points[i][1] - points[i-1][1]
    dist = math.sqrt(dx*dx + dy*dy)
    distances.append(dist)
    print(f"Distance {i-1}: {points[i-1]} -> {points[i]} = {dist:.2f}")

print(f"\nDistances: {[f'{d:.2f}' for d in distances]}")
print(f"Sorted: {sorted([f'{d:.2f}' for d in distances])}")

# After removing point at index 2 (500,500), what would the distances be?
filtered1 = [points[0], points[1], points[3], points[4], points[5], points[6], points[7]]
print(f"\nAfter removing (500,500):")
for i in range(1, len(filtered1)):
    dx = filtered1[i][0] - filtered1[i-1][0]
    dy = filtered1[i][1] - filtered1[i-1][1]
    dist = math.sqrt(dx*dx + dy*dy)
    print(f"  Distance: {filtered1[i-1]} -> {filtered1[i]} = {dist:.2f}")
