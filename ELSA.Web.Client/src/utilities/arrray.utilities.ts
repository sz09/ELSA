export function batch(items: any[], chunkSize: number) {
    if (!items) {
      return [];
    }
    var result: any[][] = [];
    for (let i = 0; i < items.length; i += chunkSize) {
      const chunk = items.slice(i, i + chunkSize);
      result.push(chunk);
    }
    return result;
  }