/**
 * Utility function for merging Tailwind CSS classes
 * Uses clsx and tailwind-merge logic
 */
export function cn(...classes: (string | boolean | undefined | null)[]): string {
  return classes
    .filter(Boolean)
    .join(' ')
    .replace(/\s+/g, ' ')
    .trim();
}

/**
 * Simple class-variance-authority implementation for Angular
 * In a real project, you would install 'class-variance-authority' package
 */
export function cva(base: string, config: any) {
  return (props: any) => {
    let classes = base;

    // Apply variants
    if (config.variants) {
      Object.keys(config.variants).forEach(key => {
        const value = props[key];
        if (value) {
          const variantClasses = config.variants[key][value];
          if (variantClasses) {
            classes += ' ' + variantClasses;
          }
        }
      });
    }

    return classes;
  };
}

export type VariantProps<T> = T extends (props: infer P) => any ? P : never;
